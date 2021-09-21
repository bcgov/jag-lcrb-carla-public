ARG MYAPP_IMAGE=registry.redhat.io/rhel8/redis-5:latest
FROM $MYAPP_IMAGE

RUN echo never > /sys/kernel/mm/transparent_hugepage/enabled
