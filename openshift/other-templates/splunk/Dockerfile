FROM splunk/splunk:latest

USER root

RUN chmod a+w /etc/passwd

copy uid_entrypoint /sbin
#copy entrypoint.sh /sbin

RUN chmod -R a+w /opt/ansible && \
  chmod -R 777 /opt/splunk

# copy ansible /opt/ansible/

# RUN chmod a-w /opt/ansible 



USER ansible

RUN /sbin/entrypoint.sh start-and-exit

### user name recognition at runtime w/ an arbitrary uid - for OpenShift deployments
ENTRYPOINT [ "uid_entrypoint" ]