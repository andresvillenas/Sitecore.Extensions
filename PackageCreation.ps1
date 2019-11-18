# Create a new Sitecore Package (SPE cmdlet)
$package = New-Package "Sitecore.Extensions"

# Set package metadata
$package.Sources.Clear();

$package.Metadata.Author = "Andrés Villenas";
$package.Metadata.Publisher = "Ecuasoft";
$package.Metadata.Version = "0.0.2.1";
$package.Metadata.Readme = 'Set of utilities for the Sitecore interface to make developers and editors more productive.'

# Get the Unicorn Configuration(s) we want to package
$configs = Get-UnicornConfiguration "Sitecore.Extensions" 

# Add the items synchronized in Unicorn
$configs | New-UnicornItemSource -Project $package

# Add the files required
$source1 = Get-ChildItem -exclude *.cs -Path "$AppPath\bin\Sitecore.Extensions.dll" -Recurse -File | New-ExplicitFileSource -Name "Files"
$package.Sources.Add($source1);

$source2 = Get-ChildItem -exclude Unicorn.* -Path "$AppPath\App_Config\Include\zSitecoreExtensions" -Recurse -File | New-ExplicitFileSource -Name "Files"
$package.Sources.Add($source2);

$source3 = Get-ChildItem -exclude *.cs -Path "$AppPath\sitecore\shell\Applications\Extensions" -Recurse -File | New-ExplicitFileSource -Name "Files"
$package.Sources.Add($source3);

# Save package
Export-Package -Project $package -Path "$($package.Name)-$($package.Metadata.Version).zip" -Zip

# Offer the user to download the package
Download-File "$SitecorePackageFolder\$($package.Name)-$($package.Metadata.Version).zip"