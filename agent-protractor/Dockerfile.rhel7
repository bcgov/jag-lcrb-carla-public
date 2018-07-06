FROM agent-nodejs-8:latest

USER 0

RUN yum -y install wget nano Xvfb  xorg-x11-xauth java-1.8.0-openjdk git && \
	wget http://dl.fedoraproject.org/pub/epel/epel-release-latest-7.noarch.rpm && \
	wget https://dl.google.com/linux/direct/google-chrome-stable_current_x86_64.rpm && \
	yum -y localinstall epel-release-latest-7.noarch.rpm && \
	yum -y localinstall google-chrome-stable_current_x86_64.rpm && \
	rm google-chrome-stable_current_x86_64.rpm && \
	yum -y install firefox; yum clean all

# Add Chrome as a user
RUN groupadd -r chrome && useradd -r -g chrome -G audio,video chrome \
    && mkdir -p /home/chrome && chown -R chrome:chrome /home/chrome && \
    dbus-uuidgen > /var/lib/dbus/machine-id

# Run Chrome non-privileged
USER chrome

CMD [ "/bin/bash" ]
