

function Invoke-Request {
    [CmdletBinding()]
    Param ([string]$uri, [PSObject]$body, [Microsoft.PowerShell.Commands.WebRequestMethod]$method, [string]$contentType="application/x-www-form-urlencoded")
   
    return Invoke-RestMethod -UseDefaultCredentials -Uri $uri -Method $method -Body $body -ContentType $contentType #-Verbose:$VerbosePreference
}
function Get-LaborDetails {
    [CmdletBinding()]
    Param ([int]$empId, [int]$FiscalPeriodId, [int]$EndNum, [int]$StartNum, [switch]$Detailed)

    $body = @{
        AliasEmployeeID=$empId
        FiscalPeriodID=$FiscalPeriodId
        LaborClassification="Scorecard"
        BeginNum=$StartNum
        EndNum=$EndNum
        OrderBy="DataSource"
    }
    $urlLaborDetails = 'http://cdp/ScorecardService/GetLaborDetailsOrderBy';

    if ($VerbosePreference -eq "Continue" -and $Detailed.IsPresent) {
        
        Write-Host "Get-LaborDetails::URL Labor Details: $urlLaborDetails" -ForegroundColor Blue
        Write-Host $($body | ConvertTo-Json) -ForegroundColor Blue
    }
    $PSCmdlet.WriteVerbose("Getting labor details from Employee ID: $empId")
    $labor =  Invoke-Request $urlLaborDetails  $body -method Post -Verbose:$VerbosePreference
    return $($labor | ? { $_.CompanyName -match '^TRIBUNAL DE JUSTICA' -or $_.CompanyName -match 'Sub_Emergency_BR_FY\d+' })
    #return $labor
}
function Get-Users {
    [CmdletBinding()]
    Param()

    return Invoke-Request -uri "http://axisbi.azurewebsites.net/api/users" -method Get
}
function Get-LaborCount {
    [CmdletBinding()]
    Param([int]$empId, [int]$FiscalPeriodId, [switch]$Detailed)

    $body = @{
        AliasEmployeeID=$empId
        FiscalPeriodID=$FiscalPeriodId
        CalculateType="Count"
    }

    $url = "http://cdp/ScorecardService/GetLaborDetailsCount"

    if ($VerbosePreference -eq "Continue" -and $Detailed.IsPresent) {
        Write-Host "Get-LaborCount::URL Labor Details: $urlLaborDetails" -ForegroundColor Blue
        Write-Host $($body | ConvertTo-Json) -ForegroundColor Blue
    }
    $PSCmdlet.WriteVerbose("Getting Labor Count from CDP")
    return $(Invoke-Request -uri $url -body $body -method Post -Verbose:$VerbosePreference ) 
}
function Post-AzureAPI{
    [CmdletBinding()]
    Param($date, $hours, $ross, $employee, [switch]$Detailed)

    $body = @{
        Ross="$ross"
        Hours=$hours
        user="$employee"
        date="$date"
    } | ConvertTo-Json

    $url = "http://axisbi.azurewebsites.net/api/ross"
    
    if ($VerbosePreference -eq "Continue" -and $Detailed.IsPresent) {
        Write-Host "Post-AzureAPI::URL: $url" -ForegroundColor Blue
        Write-Host $($body | ConvertTo-Json) -ForegroundColor Blue
    }
    Write-Host "    Engenheiro: $employee; Data: $date; Horas: $hours" -ForegroundColor Yellow

    $PSCmdlet.WriteVerbose("Sending data to AzureAPI")
    Invoke-Request -uri $url -method Post -body $body -contentType "application/json" -Verbose:$VerbosePreference
}
function Get-Scorecard {
    [CmdletBinding()]
    Param([switch]$Detailed) 

    $ScorecardHost = "cdp"
    $FiscalPeriod = [Math]::Floor(($(Get-Date) - [DateTime]"01/06/2012").totaldays / 365) * 12 + $(Get-Date).Month + 6


    $PSCmdlet.WriteVerbose("Pinging host")
    if (-not $(Test-Connection $ScorecardHost -Quiet -Count 1))
    {
        Write-Host "Host not found" -ForegroundColor Red
        return
    }

    ForEach($emp in $(Get-Users)) {
        Write-host "Começando atualização de: $($emp.Nome)" -ForegroundColor Green

        $ScoreCardCount = $(Get-LaborCount -empId $emp.AliasEmployeeId `
                                -FiscalPeriodId $FiscalPeriod `
                                -Verbose:$VerbosePreference -Detailed:$Detailed).Scorecard

        $(Get-LaborDetails -empId $emp.AliasEmployeeId -FiscalPeriodId $FiscalPeriod `
            -StartNum 0 -EndNum $ScoreCardCount `
            -Verbose:$VerbosePreference -Detailed:$Detailed ) | 
        
        Select-Object LaborEffectiveDate, LaborHours, ServiceRequestNumber |

        % { 
            $effectiveDate = ([DateTime]$_.LaborEffectiveDate).AddDays(1)
            Post-AzureAPI -date $effectiveDate -hours $_.LaborHours -ross `
                $_.ServiceRequestNumber -employee $emp.TFSUsername `
                -Verbose:$VerbosePreference -Detailed:$Detailed }
         
        $update = Invoke-Request `
            -uri "http://axisbi.azurewebsites.net/api/users/$($emp.AliasEmployeeId)" `
            -method Put `
            -body $(@{FiscalPeriod=$FiscalPeriod; Scorecard=$ScoreCardCount} | ConvertTo-Json) `
            -contentType "application/json"
        Write-host "Concluindo atualização de: $($update.Nome)" -ForegroundColor Green
        Write-Host ""

    }
}
Clear-Host
 

Get-Scorecard #-Verbose -Detailed