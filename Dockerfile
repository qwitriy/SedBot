FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY src/. /src

RUN dotnet restore
RUN dotnet publish -c Release -o /app --no-restore


# install ffmpeg
COPY --from=jrottenberg/ffmpeg:4.1-scratch / /

# # FROM ubuntu:jammy
# RUN apt-get -y update
# # Install ruby 2.3.0 for ImageMagick
# RUN apt-get -y install build-essential zlib1g-dev libssl-dev libreadline6-dev libyaml-dev
# RUN apt-get -y install wget && apt-get install -y ruby-full && ruby -v

# # Install ImageMagick
# # RUN apt-get install imagemagick -y

# # # Install ImageMagick with WEBP and HEIC support
# FROM ubuntu:latest
# ARG IMAGEMAGICK_VERSION=7.1.0-47
# RUN wget https://github.com/ImageMagick/ImageMagick/archive/refs/tags/${IMAGEMAGICK_VERSION}.tar.gz 
# #https://download.imagemagick.org/ImageMagick/download/releases/ImageMagick-7.1.0-31.tar.gz
# RUN tar -xvf ${IMAGEMAGICK_VERSION}.tar.gz
# WORKDIR /app/${IMAGEMAGICK_VERSION}/
# RUN ./configure --with-heic=yes --with-webp=yes
# RUN make
# RUN make install
# RUN ldconfig /usr/local/lib
# RUN identify --version


# final stage/image
FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /app
COPY --from=build /app .
ARG BOT_TOKEN
ENTRYPOINT ["./SedBot"]
#build: docker build -t sedbot .
#run:   docker run sedbot token