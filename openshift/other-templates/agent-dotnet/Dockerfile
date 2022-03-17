ARG MYAPP_IMAGE=registry.redhat.io/openshift4/ose-jenkins-agent-base
FROM $MYAPP_IMAGE

# Source Image:
# https://catalog.redhat.com/software/containers/openshift4/ose-jenkins-agent-base
# registry.redhat.io/openshift4/ose-jenkins-agent-base

# Labels consumed by Red Hat build service
LABEL com.redhat.component="rh-dotnet31-jenkins-slave-container" \
      name="dotnet/dotnet-31-jenkins-slave-rhel7" \
      version="5.0" \
      architecture="x86_64" \
      release="1" \
      io.k8s.display-name="Jenkins Slave .NET Core 5.0" \
      io.k8s.description="The jenkins slave dotnet image has the dotnet tools on top of the jenkins slave base image." \
      io.openshift.tags="openshift,jenkins,slave,dotnet,dotnet31"

# Don't download/extract docs for nuget packages
# Enable nodejs and dotnet scl
ENV DOTNET_CORE_VERSION=5.0 \
    TZ=GMT-7 \
    BASH_ENV=/usr/local/bin/scl_enable \
    ENV=/usr/local/bin/scl_enable \
    PROMPT_COMMAND=". /usr/local/bin/scl_enable" \
    ENABLED_COLLECTIONS="rh-nodejs10 rh-dotnet50" \
    NUGET_XMLDOC_MODE=skip

COPY contrib/bin/scl_enable /usr/local/bin/scl_enable

# Install
RUN INSTALL_PKGS="dotnet-sdk-5.0.x86_64 nodejs.x86_64 nodejs-devel.x86_64 npm.x86_64" && \
    yum install -y --setopt=tsflags=nodocs --disablerepo=\* \
     --disableplugin=subscription-manager --enablerepo=ubi-8-appstream,ubi-8-baseos,ubi-8-codeready-builder \
      $INSTALL_PKGS && \
    rpm -V $INSTALL_PKGS && \
    yum clean all -y && \
# yum cache files may still exist (and quite large in size)
    rm -rf /var/cache/yum/*

# Trigger first time actions.
# RUN scl enable dotnet-sdk-3.1.x86_64 'dotnet help'

RUN chown -R 1001:0 $HOME && \
    chmod -R g+rw $HOME

# may also need xorg-x11-server-Xvfb x11vnc

RUN yum -y install --disableplugin=subscription-manager --enablerepo=ubi-8-appstream,ubi-8-baseos,ubi-8-codeready-builder xorg-x11-server-utils.x86_64 java-1.8.0-openjdk-devel.x86_64   && \   
	wget http://dl.fedoraproject.org/pub/epel/epel-release-latest-7.noarch.rpm && \
	wget https://dl.google.com/linux/direct/google-chrome-stable_current_x86_64.rpm && \
	yum -y localinstall epel-release-latest-7.noarch.rpm && \
	yum --enablerepo=ubi-8-appstream,ubi-8-baseos,ubi-8-codeready-builder --disableplugin=subscription-manager -y install https://rpmfind.net/linux/centos/8-stream/AppStream/x86_64/os/Packages/desktop-file-utils-0.23-8.el8.x86_64.rpm && \
	yum --enablerepo=ubi-8-appstream,ubi-8-baseos,ubi-8-codeready-builder --disableplugin=subscription-manager -y install https://rpmfind.net/linux/centos/8-stream/AppStream/x86_64/os/Packages/xdg-utils-1.1.2-5.el8.noarch.rpm && \
	yum --enablerepo=ubi-8-appstream,ubi-8-baseos,ubi-8-codeready-builder --disableplugin=subscription-manager -y install http://vault.centos.org/centos/8/AppStream/x86_64/os/Packages/llvm-libs-11.0.0-2.module_el8.4.0+587+5187cac0.x86_64.rpm http://vault.centos.org/centos/8/AppStream/x86_64/os/Packages/llvm-11.0.0-2.module_el8.4.0+587+5187cac0.x86_64.rpm && \
	yum --enablerepo=ubi-8-appstream,ubi-8-baseos,ubi-8-codeready-builder --disableplugin=subscription-manager -y install http://rpmfind.net/linux/centos/7.9.2009/os/x86_64/Packages/vulkan-filesystem-1.1.97.0-1.el7.noarch.rpm && \
	yum --enablerepo=ubi-8-appstream,ubi-8-baseos,ubi-8-codeready-builder --disableplugin=subscription-manager -y install http://rpmfind.net/linux/centos/7.9.2009/os/x86_64/Packages/vulkan-1.1.97.0-1.el7.x86_64.rpm && \
	yum --enablerepo=ubi-8-appstream,ubi-8-baseos,ubi-8-codeready-builder --disableplugin=subscription-manager -y install http://vault.centos.org/centos/8/AppStream/x86_64/os/Packages/mesa-vulkan-drivers-20.3.3-2.el8.x86_64.rpm http://vault.centos.org/centos/8/AppStream/x86_64/os/Packages/vulkan-loader-1.2.162.0-1.el8.x86_64.rpm && \
	yum --enablerepo=ubi-8-appstream,ubi-8-baseos,ubi-8-codeready-builder --disableplugin=subscription-manager -y install https://vault.centos.org/centos/8/BaseOS/x86_64/os/Packages/liberation-fonts-common-2.00.3-7.el8.noarch.rpm http://vault.centos.org/centos/8/BaseOS/x86_64/os/Packages/liberation-mono-fonts-2.00.3-7.el8.noarch.rpm http://vault.centos.org/centos/8/BaseOS/x86_64/os/Packages/liberation-sans-fonts-2.00.3-7.el8.noarch.rpm http://mirror.centos.org/centos/8/BaseOS/x86_64/os/Packages/liberation-serif-fonts-2.00.3-7.el8.noarch.rpm http://mirror.centos.org/centos/8/BaseOS/x86_64/os/Packages/liberation-fonts-2.00.3-7.el8.noarch.rpm && \
	yum --enablerepo=ubi-8-appstream,ubi-8-baseos,ubi-8-codeready-builder --disableplugin=subscription-manager -y localinstall google-chrome-stable_current_x86_64.rpm && \
	rm google-chrome-stable_current_x86_64.rpm && \
	yum clean all -y && \
    rm -rf /var/cache/yum/* 

RUN echo ${PATH}

# RUN ls /opt/rh/rh-nodejs10/root/usr/lib/

# ENV PATH="/opt/rh/rh-nodejs10/root/usr/bin:/opt/app-root/node_modules/.bin:${PATH}"

#RUN npm install -g @angular/cli && \
#   npm install -g typescript

#	yum -y install firefox; yum clean all

# Add Chrome as a user
RUN groupadd -r chrome && useradd -r -g chrome -G audio,video chrome \
    && mkdir -p /home/chrome && chown -R chrome:chrome /home/chrome && \
    dbus-uuidgen > /var/lib/dbus/machine-id

ENV DOTNET_SSL_DIRS=/var/run/secrets/kubernetes.io/serviceaccount/service-ca.crt

# Run Chrome non-privileged
USER chrome

CMD [ "/bin/bash" ]
