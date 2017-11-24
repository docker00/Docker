FROM python:3.6-stretch

LABEL \
    version="1.0" \
    author="Mikhail Zakharov <zakharov@scanex.ru>" \
    maintainer="Mikhail Velkin <twilight@solfoto.ru>" \
    description="Python Demo Application" \
    source="https://github.com/ScanEx/"

#
# Setting proxy
#
#ENV http_proxy='http://192.168.4.10:8080' \
#    https_proxy='https://192.168.4.10:8080'

#
# Copy the current directory contents into the container at /app
#
COPY ./assets/src /app

RUN set -ex \
#
# System Update
#
 && apt-get update -yqq \
 && apt-get install --no-install-recommends -yqq \
    # Base Dependencies
      apt-utils \
      build-essential \
      ca-certificates \
      curl \
      git \
      python3-dev \
      python3-pip \
      python3-requests \
      unzip \
      wget \
#
# Installing Python Dependencies
#
 && python -m pip install -U -r /app/requirements.txt \
#
# Clean all sources and cache and temporary files
#
 && rm -rf \
      /usr/src/* \
      /var/lib/apt/lists/* \
      /tmp/* \
      /var/tmp/* \
      /usr/share/man \
      /usr/share/doc \
      /usr/share/doc-base

#
# Make port available to the world outside that container
#
EXPOSE 80

#
# Set the working directory to /app
#
WORKDIR /app

#
# Run the application after launching the container
#
CMD ["python", "test.py"]
