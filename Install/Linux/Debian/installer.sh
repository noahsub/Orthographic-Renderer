# version number variable
# read the version number from the version file
version=$(<../../../VERSION)
# package name variable
name="orthographic-renderer_$version"

# Remove the tmp directory if it exists
rm -rf tmp

# Remove the .deb file if it exists
rm -f orthographic-renderer_$version.deb

# Create tmp Directory in current directory
mkdir -p tmp

# Create required directories
mkdir -p tmp/usr
mkdir -p tmp/usr/local
mkdir -p tmp/usr/local/bin
mkdir -p tmp/usr/local/bin/orthographic-renderer
mkdir -p tmp/usr/share
mkdir -p tmp/usr/share/applications

# publish the dotnet core application
cd ../../..
dotnet publish -c Release -r linux-x64 --self-contained -o ./Install/Linux/Debian/tmp/usr/local/bin/orthographic-renderer
cd Install/Linux/Debian

# Create the DEBIAN directory
mkdir -p tmp/DEBIAN

# Create the control file
touch tmp/DEBIAN/control
echo "Package: orthographic-renderer" > tmp/DEBIAN/control
echo "Version: $version" >> tmp/DEBIAN/control
echo "Maintainer: noahsub" >> tmp/DEBIAN/control
echo "Architecture: amd64" >> tmp/DEBIAN/control
echo "Description: A tool for rendering orthographic views of 3D models, designed to replace traditional CPU rendering in CAD software. It is optimized for both speed and quality, featuring parallel rendering capabilities and GPU acceleration via OPTIX and CUDA." >> tmp/DEBIAN/control

# Create the desktop file
touch tmp/usr/share/applications/orthographic-renderer.desktop
echo "[Desktop Entry]" > tmp/usr/share/applications/orthographic-renderer.desktop
echo "Version=$version" >> tmp/usr/share/applications/orthographic-renderer.desktop
echo "Name=Orthographic Renderer" >> tmp/usr/share/applications/orthographic-renderer.desktop
echo "Exec=/usr/local/bin/orthographic-renderer/Orthographic\ Renderer" >> tmp/usr/share/applications/orthographic-renderer.desktop
echo "Path=/usr/local/bin/orthographic-renderer" >> tmp/usr/share/applications/orthographic-renderer.desktop
echo "Icon=/usr/local/bin/orthographic-renderer/Assets/Icons/green_cube.png" >> tmp/usr/share/applications/orthographic-renderer.desktop
echo "Type=Application" >> tmp/usr/share/applications/orthographic-renderer.desktop
echo "Categories=Graphics;3DGraphics;" >> tmp/usr/share/applications/orthographic-renderer.desktop

# Build the package
dpkg-deb --build tmp orthographic-renderer_$version.deb

# Remove the tmp directory
rm -rf tmp