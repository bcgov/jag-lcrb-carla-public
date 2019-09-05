FROM splunk/splunk:latest

USER root

RUN chmod a+w /etc/passwd

copy uid_entrypoint /sbin
# copy entrypoint.sh /sbin

RUN mkdir -p /opt/splunk/etc && \
  mkdir -p /opt/container_artifact && \
  chmod a+w /opt/container_artifact && \
  chmod a+w /opt/splunk/etc && \
  chmod -R a+r /opt/splunk-etc && \
  chmod -R 777 /opt/splunk && \
  chmod -R a+rx /opt/ansible

USER ansible

RUN /sbin/entrypoint.sh start-and-exit

RUN cp -ar /opt/splunk/etc /opt/splunk/etc-default && \
  chmod -R a+rx /opt/ansible

### user name recognition at runtime w/ an arbitrary uid - for OpenShift deployments
ENTRYPOINT [ "uid_entrypoint" ]