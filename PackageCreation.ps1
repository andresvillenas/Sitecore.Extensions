# Create a new Sitecore Package (SPE cmdlet)
$package = New-Package "Sitecore.Extensions"

# Set package metadata
$package.Sources.Clear();

$package.Metadata.Author = "Andrés Villenas";
$package.Metadata.Publisher = "Ecuasoft";
$package.Metadata.Version = "0.0.2.0";
$package.Metadata.Readme = 'Set of utilities for the Sitecore interface to make developers and editors more productive.'

# Get the Unicorn Configuration(s) we want to package
$configs = Get-UnicornConfiguration "Sitecore.Extensions" 

# Pipe the configs into New-UnicornItemSource 
# to process them and add them to the package project
# (without -Project, this would emit the source object(s) 
#   which can be manually added with $pkg.Sources.Add())
$configs | New-UnicornItemSource -Project $package

# Add content of the Console folder (except the source files) located in the site folder to the package
$source = Get-ChildItem -exclude *.cs -Path "$AppPath\bin\Sitecore.Extensions.dll" -Recurse -File | New-ExplicitFileSource -Name "DLLs"
$package.Sources.Add($source);

# Save package
Export-Package -Project $package -Path "$($package.Name)-$($package.Metadata.Version).zip" -Zip

# Offer the user to download the package
Download-File "$SitecorePackageFolder\$($package.Name)-$($package.Metadata.Version).zip"