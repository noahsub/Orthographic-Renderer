# version number variable
# read the version number from the version file
version=$(<../../../VERSION)
version=${version:1}
# package name variable
name="orthographic-renderer_$version"

# Remove the tmp directory if it exists
rm -rf tmp

# Remove the .deb file if it exists
rm -f orthographic-renderer_$version.deb

# Create tmp Directory in current directory
mkdir -p tmp/usr/local/bin/orthographic-renderer
mkdir -p tmp/usr/share/applications

# publish the dotnet core application
cd ../../..
dotnet publish -c Release -r linux-x64 --self-contained -o ./Install/Linux/Debian/tmp/usr/local/bin/orthographic-renderer
cd Install/Linux/Debian

# Make the Blender directory
cd tmp/usr/local/bin/orthographic-renderer
mkdir -p Blender/Linux

# Download Blender
cd ..
curl -fSLo ./blender-4.2.4-linux-x64.tar.xz https://mirrors.ocf.berkeley.edu/blender/release/Blender4.2/blender-4.2.4-linux-x64.tar.xz

# Extract Blender
tar -xf blender-4.2.4-linux-x64.tar.xz -C .
mv blender-4.2.4-linux-x64/* orthographic-renderer/Blender/Linux

# Remove archive and empty directory
rm -rf blender-4.2.4-linux-x64
rm blender-4.2.4-linux-x64.tar.xz

# Go back to the Install/Linux/Debian directory
cd ../../../../

# Create the DEBIAN directory
mkdir -p tmp/DEBIAN

# Create the control file
cat > tmp/DEBIAN/control <<EOL
Package: orthographic-renderer
Version: $version
Maintainer: noahsub
Architecture: amd64
Description: A tool for rendering orthographic views of 3D models, designed to replace traditional CPU rendering in CAD software. It is optimized for both speed and quality, featuring parallel rendering capabilities and GPU acceleration via OPTIX and CUDA.
EOL

# Make the binary executable
chmod +x tmp/usr/local/bin/orthographic-renderer/Orthographic\ Renderer

# Create the desktop file (fixed)
cat > tmp/usr/share/applications/orthographic-renderer.desktop <<EOL
[Desktop Entry]
Version=$version
Name=Orthographic Renderer
Exec="/usr/local/bin/orthographic-renderer/Orthographic Renderer"
Icon=/usr/local/bin/orthographic-renderer/Assets/Icons/green_cube.png
Type=Application
Terminal=false
Categories=Graphics;3DGraphics;
EOL

# Build the package
dpkg-deb --build tmp orthographic-renderer_$version.deb

# Remove the tmp directory
rm -rf tmp
