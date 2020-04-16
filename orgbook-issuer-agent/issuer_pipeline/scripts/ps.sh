docker run --name event_processor -p 5444:5432 -e POSTGRES_USER=ep_user -e POSTGRES_PASSWORD=ep_pass -e POSTGRES_DB=EVENT_PROCESSOR -d postgres


