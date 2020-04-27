from data_integration.pipelines import Pipeline, Task
from data_integration.commands.python import ExecutePython

def von_root_pipeline():

    parent_pipeline = Pipeline(
        id = 'holder_for_pipeline_versions',
        description = 'Holder for the different versions of the VON Data Pipeline.')

    parent_pipeline.add(von_data_pipeline())
    parent_pipeline.add(von_data_pipeline_status())

    init_pipeline = Pipeline(
        id = 'initialization_and_load_tasks',
        description = 'One-time initialization and data load tasks')

    init_pipeline.add(db_init_pipeline())

    parent_pipeline.add(init_pipeline)

    test_pipeline = Pipeline(
        id = 'test_and_demo_tasks',
        description = 'Holder for test and demo tasks.')

    test_pipeline.add(von_data_test_registrations())

    parent_pipeline.add(test_pipeline)

    return parent_pipeline

def von_data_pipeline():
    import von_pipeline

    pipeline1 = Pipeline(
        id='von_data_event_processor',
        description='A pipeline that processes von_data events and generates credentials.')

    sub_pipeline1_2 = Pipeline(id='load_and_process_von_data_data', description='Load von_data data and generate credentials')
    sub_pipeline1_2.add(Task(id='create_von_data_credentials', description='Create credentials',
                          commands=[ExecutePython('./von_pipeline/generate-creds.py')]))
    pipeline1.add(sub_pipeline1_2)

    sub_pipeline1_3 = Pipeline(id='submit_von_data_credentials', description='Submit von_data credentials to P-X')
    sub_pipeline1_3.add(Task(id='submit_credentials', description='Submit credentials',
                          commands=[ExecutePython('./von_pipeline/submit-creds.py')]))
    pipeline1.add(sub_pipeline1_3, ['load_and_process_von_data_data'])

    return pipeline1

def von_data_pipeline_status():
    import von_pipeline

    pipeline = Pipeline(
        id='von_data_pipeline_status',
        description='Display overall event processing status.')

    pipeline.add(Task(id='display_pipeline_status', description='Display status of the overall pipeline processing status',
                        commands=[ExecutePython('./von_pipeline/display_pipeline_status.py')]))

    return pipeline

def db_init_pipeline():
    import von_pipeline

    pipeline = Pipeline(
      id = 'von_data_db_init',
      description = 'Initialize von_data Event Processor database')

    pipeline.add(Task(id='create_tables', description='Create event processing tables',
                        commands=[ExecutePython('./von_pipeline/create.py')]))
    pipeline.add(Task(id='initialize_tables', description='Insert configuration data',
                        commands=[ExecutePython('./von_pipeline/insert.py')]), ['create_tables'])

    return pipeline

def von_data_test_registrations():
    import von_pipeline

    pipeline = Pipeline(
        id='von_data_test_registrations',
        description='A pipeline that queues up a small set of test corporations.')

    pipeline.add(Task(id='register_test_corps', description='Register some test corps for processing',
                        commands=[ExecutePython('./von_pipeline/find-test-corps.py')]))

    return pipeline

def von_list_mongo_data():
    import von_pipeline

    pipeline = Pipeline(
        id='von_list_mongo_data',
        description='A pipeline that lists data in mongodb.')

    pipeline.add(Task(id='list_mongo_data', description='List data queued for processing',
                        commands=[ExecutePython('./von_pipeline/list_mongo_data.py')]))

    return pipeline
