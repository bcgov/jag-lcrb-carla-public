FROM splunk/splunk:latest

USER root

RUN chmod a+w /etc/passwd

copy uid_entrypoint /sbin
#copy entrypoint.sh /sbin


RUN mkdir -p /opt/splunk/etc && \
  chmod a+w /opt/splunk/etc

# copy ansible /opt/ansible/

# RUN chmod a-w /opt/ansible 

RUN chmod -R 777 /opt/splunk

USER ansible

RUN /sbin/entrypoint.sh start-and-exit

RUN chmod -R a+rx /opt/ansible


### user name recognition at runtime w/ an arbitrary uid - for OpenShift deployments
ENTRYPOINT [ "uid_entrypoint" ]