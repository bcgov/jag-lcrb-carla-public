FROM splunk/splunk:latest

# copy getpass.py /usr/lib/python2.7

USER root

RUN chmod a+w /etc/passwd

copy uid_entrypoint /sbin


USER ansible

### user name recognition at runtime w/ an arbitrary uid - for OpenShift deployments
ENTRYPOINT [ "uid_entrypoint" ]