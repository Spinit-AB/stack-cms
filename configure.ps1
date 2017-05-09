param([string] $name, [Int] $port, [string] $catalog, [string] $datasource, [switch] $testdb)

$currentName = 'Spinit.Stack.CMS'

function Clean-Site()
{
	# Remove bin and obj folders
	Get-ChildItem . -recurse | ?{ $_.fullname -match "\\(bin|obj)(\\.*)?$" } | Inform -text 'Removing: ' | Remove-Item -Recurse
}

function Edit-ConnectionStrings($catalog, $datasource, $testdb)
{
	Get-ChildItem . -recurse | ?{ $_.fullname -notmatch "\\packages\\?" } | ?{ $_.fullname -notmatch "\\.hg\\?" } | ?{ $_.fullname -notmatch "\\.build\\?" } | ?{ $_.Name -eq 'Web.config' -or $_.Name -eq 'app.config' } | ?{ (($testdb -and $_.DirectoryName.EndsWith('.Tests')) -or (-not $testdb -and -not $_.DirectoryName.EndsWith('.Tests'))) } | % { Edit-ConnectionString -fileInfo $_ -catalog $catalog -datasource $datasource }
}

function Edit-ConnectionString($fileInfo, $catalog, $datasource)
{
	$cfg = [xml](Get-Content $fileInfo.FullName)
				
	if($cfg.configuration.connectionStrings -ne $null)
	{
        $m = $null
		$m = ($cfg.configuration.connectionStrings.add | ? { $_.name -eq $env:computername})
		if($m -eq $null)
		{
			$xmlElt = $cfg.CreateElement("add")
			
			$xmlNameAtt = $cfg.CreateAttribute("name")
			$xmlNameAtt.Value = $env:computername
			$xmlElt.Attributes.Append($xmlNameAtt)
			
			$xmlProviderAtt = $cfg.CreateAttribute("providerName")
			$xmlProviderAtt.Value = 'System.Data.SqlClient'
			$xmlElt.Attributes.Append($xmlProviderAtt)
			
			$xmlConnectionStringAtt = $cfg.CreateAttribute("connectionString")
			$xmlConnectionStringAtt.Value = 'Data Source='+$datasource+';Initial Catalog='+$catalog+';Integrated Security=SSPI;'
			$xmlElt.Attributes.Append($xmlConnectionStringAtt)
			
			$cfg.configuration.connectionStrings.AppendChild($xmlElt)
			"Updated connectionstring in " + $fileInfo.FullName
		}
		else{
			$m | % { 
				if($catalog)
				{
					$_.connectionString = $_.connectionString -replace 'Initial Catalog=[a-zA-Z0-9_]*;?', ('Initial Catalog='+$catalog+';');
					"Updated Initial Catalog in " + $fileInfo.FullName
				}
				if($datasource)
				{
					$_.connectionString = $_.connectionString -replace 'Data Source=[a-zA-Z0-9_\.\\]*;?', ('Data Source='+$datasource+';');
					"Updated Data Source in " + $fileInfo.FullName
				}
			}
		}
		$cfg.Save($fileInfo.FullName)
	}
}

function Edit-ProjectName($search, $name)
{
	# Update content of files
	Get-ChildItem . -recurse | ?{ $_.fullname -notmatch "\\packages\\?" } | ?{ $_.fullname -notmatch "\\.hg\\?" } | ?{ $_.fullname -notmatch "\\.build\\?" } | ?{ $_.GetType().Name -eq 'FileInfo'} | % { ReplaceText -fileInfo $_ -search $search -name $name }
	
	# Rename files and folders
	Get-ChildItem . -recurse | ?{ $_.fullname -notmatch "\\packages\\?" } | ?{ $_.fullname -notmatch "\\.hg\\?" } | ?{ $_.fullname -notmatch "\\.build\\?" } | ?{ $_.fullname -match $search } | sort -Property @{ Expression = {$_.FullName.Split('\').Count} } -Desc | % { Rename-File -fileInfo $_ -search $search -name $name}
}

function Rename-File($fileInfo, $search, $name)
{
	$newName = $fileInfo.Name.replace($search, $name)
	if($newName -eq $fileInfo.Name){
		return;
	}
	rename-item $fileInfo.FullName $newName
	"Renamed: " + $newName
}

function Inform($text)
{
	PROCESS {
		Write-Host ($text+$_.FullName)
		$_
	}
	
}

function ReplaceText($fileInfo, $search, $name)
{    
	(Get-Content $fileInfo.FullName) | % {$_ -replace $search, $name} | Set-Content -path $fileInfo.FullName
	"Processed: " + $fileInfo.FullName
   
}

function UpdatePort($fileInfo, $port)
{
	if( $fileInfo.Name.EndsWith('.sln'))
	{
		(Get-Content $fileInfo.FullName) | % {$_ -replace "localhost:\d*", "localhost:${port}"} | Set-Content -path $fileInfo.FullName
		"Processed: " + $fileInfo.FullName
	}
	
	if( $fileInfo.Name.EndsWith('.Api.csproj'))
	{
		(Get-Content $fileInfo.FullName) | % {$_ -replace "localhost:\d*", "localhost:${port}"} | Set-Content -path $fileInfo.FullName
		"Processed: " + $fileInfo.FullName
	}
	
	if( $fileInfo.Name -eq 'site.xml')
	{
		(Get-Content $fileInfo.FullName) | % {$_ -replace ":\d*:localhost", ":${port}:localhost" } | Set-Content -path $fileInfo.FullName
		"Processed: " + $fileInfo.FullName
	}
}

function Get-ApplicationHostConfigPath()
{
    $path = [environment]::getfolderpath("mydocuments")+'\IISExpress\config\applicationhost.config'
	$fileInfo = Get-Item -path $path
	return $fileInfo
	
}

function Get-Xml($fileInfo)
{
    return [xml](Get-Content $fileInfo.FullName)
}

function Update-ApplicationHostConfig($search, $name)
{
	$fileInfo = Get-ApplicationHostConfigPath
    $cfg = Get-Xml -fileInfo $fileInfo
    
	$m = $null
	$m = $cfg.configuration.location | ? { $_.path -eq "$search"}
	
	if($m -eq $null)
	{
		Create-Location -cfg $cfg -name "$name"	
		"Added location to " + $fileInfo.FullName
	}
	else
	{
		$m | % { 
			$_.path = $_.path -replace $search, $name;
			"Updated location in " + $fileInfo.FullName
			
		}
	}
	
	$cfg.Save($fileInfo.FullName)
}

function Find-FirstMissingNumberInArray($integers)
{
    $lastFound = 0;
    $firstMissingNumber = 0;
    $lastCandidate = 0
    $integers | Sort-Object {[int]$_} | 
    %{ 
        $lastCandidate = [int]$_
        if($firstMissingNumber -eq 0)
        {
            if([int]$_ -eq $lastFound -or [int]$_ -eq ($lastFound+1))
            {
                $lastFound = [int]$_ + 0
            }
            else
            {
                $firstMissingNumber = $lastFound + 1
            }
        }
    }
    if($firstMissingNumber -eq 0)
    {
        $firstMissingNumber = $lastCandidate + 1;
    }

    return $firstMissingNumber
}

function Create-Location($cfg, $name)
{
	$location = $cfg.CreateElement("location")
		
	$pathAtt = $cfg.CreateAttribute("path")
	$pathAtt.Value = $name
	$location.Attributes.Append($pathAtt)
	
	$sw = $cfg.CreateElement("system.webServer")
	$location.AppendChild($sw)
	
	$security = $cfg.CreateElement("security")
	$sw.AppendChild($security)
	
	$authentication = $cfg.CreateElement("authentication")
	$security.AppendChild($authentication)
	
	$anonymousAuthentication = $cfg.CreateElement('anonymousAuthentication')
	$authentication.AppendChild($anonymousAuthentication)
	
	$anonymousEnabled = $cfg.CreateAttribute("enabled")
	$anonymousEnabled.Value = "true"
	$anonymousAuthentication.Attributes.Append($anonymousEnabled)
	
	$windowsAuthentication = $cfg.CreateElement('windowsAuthentication')
	$authentication.AppendChild($windowsAuthentication)
	 
	$windowsEnabled = $cfg.CreateAttribute("enabled")
	$windowsEnabled.Value = "false"
	$windowsAuthentication.Attributes.Append($windowsEnabled)
	
	$cfg.configuration.AppendChild($location)
}

function Create-ApplicationXmlElement($path, $parent, $name)
{
                $xmlApp1Elm = $cfg.CreateElement("application")

        $xmlApp1PathAtt = $cfg.CreateAttribute("path")
		$xmlApp1PathAtt.Value = $path
		$xmlApp1Elm.Attributes.Append($xmlApp1PathAtt)

        $xmlApp1AppPoolAtt = $cfg.CreateAttribute("applicationPool")
		$xmlApp1AppPoolAtt.Value = 'Clr4IntegratedAppPool'
		$xmlApp1Elm.Attributes.Append($xmlApp1AppPoolAtt)

        $xmlVirtDirElm = $cfg.CreateElement('virtualDirectory')

        $xmlVirtDirPathAtt = $cfg.CreateAttribute("path")
		$xmlVirtDirPathAtt.Value = '/'
		$xmlVirtDirElm.Attributes.Append($xmlVirtDirPathAtt)

        $xmlVirtDirPhysPathAtt = $cfg.CreateAttribute("physicalPath")
		$xmlVirtDirPhysPathAtt.Value = $PSScriptRoot + '\' + $name
		$xmlVirtDirElm.Attributes.Append($xmlVirtDirPhysPathAtt)

        $xmlApp1Elm.AppendChild($xmlVirtDirElm)
        $parent.AppendChild($xmlApp1Elm)
}

function Update-Site($search, $name, $port)
{
    $fileInfo = Get-ApplicationHostConfigPath
    $cfg = Get-Xml -fileInfo $fileInfo
    $firstFreeSiteId = Find-FirstMissingNumberInArray -integers $cfg.configuration.'system.applicationHost'.sites.site.id

    $site = $null
    $site = $cfg.configuration.'system.applicationHost'.sites.site | ? { $_.name -eq $search}
	if($site -ne $null)
    {
        $site.name = $site.name -replace ($search), ($name)
        $site.application | ? { $_.path -eq '/'} | %{ $_.virtualDirectory.physicalPath = $_.virtualDirectory.physicalPath -replace ($search), ($name)}
        $site.bindings.binding.bindingInformation = ("*:"+$port+":localhost") 
		Write-Host "Updated site in applicationhost.config"
    }
    else
    {
        $xmlSiteElm = $cfg.CreateElement("site")
			
		$xmlIdAtt = $cfg.CreateAttribute("id")
		$xmlIdAtt.Value = $firstFreeSiteId
		$xmlSiteElm.Attributes.Append($xmlIdAtt)

        $xmlNameAtt = $cfg.CreateAttribute("name")
		$xmlNameAtt.Value = $name
		$xmlSiteElm.Attributes.Append($xmlNameAtt)

        Create-ApplicationXmlElement -path '/' -parent $xmlSiteElm -name ($name)
     
        $xmlBindingsElm = $cfg.CreateElement("bindings")
        $xmlBindingElm = $cfg.CreateElement("binding")

        $xmlProtAtt = $cfg.CreateAttribute("protocol")
		$xmlProtAtt.Value = 'http'
		$xmlBindingElm.Attributes.Append($xmlProtAtt)

        $xmlInfoAtt = $cfg.CreateAttribute("bindingInformation")
		$xmlInfoAtt.Value = '*:'+$port+':localhost'
		$xmlBindingElm.Attributes.Append($xmlInfoAtt)

        $xmlBindingsElm.AppendChild($xmlBindingElm)
        $xmlSiteElm.AppendChild($xmlBindingsElm)

		$cfg.configuration.'system.applicationHost'.sites.AppendChild($xmlSiteElm)
		Write-Host "Added site to applicationHost.config"
    }

    $cfg.Save($fileInfo)
}

if($catalog -or $datasource)
{
	Edit-ConnectionStrings -catalog $catalog -datasource $datasource -testdb $testdb
}

if($name)
{
    Clean-Site

	if($name -ne $currentName)
	{
		Edit-ProjectName -search $currentName -name $name
	
		# Update-ApplicationHostConfig -search $currentName -name $name
	}
	
    # Update-Site -search $currentName -name $name -port $port 

    Get-ChildItem . -recurse | ?{ $_.fullname -notmatch "\\packages\\?" } | ?{ $_.fullname -notmatch "\\.hg\\?" } | ?{ $_.fullname -notmatch "\\.build\\?" } | ?{ $_.GetType().Name -eq 'FileInfo'} | % { UpdatePort -fileInfo $_ -port $port }	
}
