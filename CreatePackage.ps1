$package = New-Package "Sitecore Crownpeak Connector"

$package.Metadata.Author = "Cognifide";
$package.Metadata.Publisher = "Cognifide Limited";
$package.Metadata.Version = "2.0.0";

$configs = Get-UnicornConfiguration "Cognifide.Sitecore.Crownpeak" 

$configs | New-UnicornItemSource -Project $package

# Files
$source = Get-Item "$AppPath\bin\Cognifide.Sitecore.Crownpeak.dll" | New-ExplicitFileSource -Name "Assembly"
$package.Sources.Add($source);
$source = Get-Item "$AppPath\App_Config\Include\Cognifide.Crownpeak.config" | New-ExplicitFileSource -Name "Configuration File"
$package.Sources.Add($source);
$source = Get-ChildItem -exclude *.cs -Path "$AppPath\Content\crownpeak" -Recurse -File | New-ExplicitFileSource -Name "Assets"
$package.Sources.Add($source);
$source = Get-ChildItem -exclude *.cs -Path "$AppPath\sitecore\shell\Applications\Content Manager\Editors\Crownpeak" -Recurse -File | New-ExplicitFileSource -Name "Editors"
$package.Sources.Add($source);
$source = Get-ChildItem -exclude *.cs -Path "$AppPath\sitecore\shell\Applications\Content Manager\Controls\Crownpeak" -Recurse -File | New-ExplicitFileSource -Name "Dialogs"
$package.Sources.Add($source);

Export-Package -Project $package -Path "$($package.Name)-$($package.Metadata.Version).xml"
Export-Package -Project $package -Path "$($package.Name)-$($package.Metadata.Version).zip" -Zip
Download-File "$SitecorePackageFolder\$($package.Name)-$($package.Metadata.Version).zip"