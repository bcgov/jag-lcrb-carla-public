from pymongo import MongoClient

import datetime
import pytz
import json
import string
import decimal
import random
import types

from von_pipeline.config import config


###########################################################################
# random string methods (for generating test data)
###########################################################################

def random_alpha_string(length, contains_spaces=False):
    if contains_spaces:
        chars = string.ascii_uppercase + ' '
    else:
        chars = string.ascii_uppercase
    return ''.join(random.SystemRandom().choice(chars) for _ in range(length))

def random_numeric_string(length):
    chars = string.digits
    return ''.join(random.SystemRandom().choice(chars) for _ in range(length))

def random_an_string(length, contains_spaces=False):
    if contains_spaces:
        chars = string.ascii_uppercase + string.digits + ' '
    else:
        chars = string.ascii_uppercase + string.digits
    return ''.join(random.SystemRandom().choice(chars) for _ in range(length))

def random_date_days(old_day_range, new_day_range):
    now = datetime.datetime.today()
    days = random.randint(old_day_range, new_day_range)
    return now + datetime.timedelta(days = days)

def random_date_dates(old_date, new_date):
    now = datetime.datetime.today()
    return random_date_days((today - old_date).days, (today - new_date).days)


###########################################################################
# random object methods (for generating test data)
###########################################################################

def gen_user():
    user = {
        "_updated_at":random_date_days(-5, -1),
        "_created_at":random_date_days(-20, -5),
        "username":random_alpha_string(20),
        "email":random_alpha_string(20) + '@' + random_alpha_string(12) + '.' + random_alpha_string(3),
        "emailVerified":True,
        "lastName":random_alpha_string(20),
        "hasLoggedIn":True,
        "permission":random_alpha_string(20),
        "isActive":True,
        "publicEmail":random_alpha_string(20) + '@' + random_alpha_string(12) + '.' + random_alpha_string(3),
        "firstName":random_alpha_string(8),
    }
    return user

def gen_team(user):
    team = {
        "_updated_at":random_date_days(-5, -1),
        "_created_at":random_date_days(-20, -5),
        "color":random_alpha_string(8),
        "name":random_alpha_string(20),
        "isActive":True,
    }
    return team

def gen_inspection(user, team):
    inspection = {
        "teamID":team['_id'],
        "_updated_at":random_date_days(-5, -1),
        "_created_at":random_date_days(-20, -5),
        "_uploaded_at":random_date_days(-3, -1),
        "_uploaded_hash":random_alpha_string(40),
        "start":random_date_days(-20, -5),
        "number":random_numeric_string(8),
        "subtext":random_alpha_string(40, contains_spaces=True),
        "end":random_date_days(-5, -1),
        "userId":user['_id'],
        "subtitle":random_alpha_string(40, contains_spaces=True),
        "title":random_alpha_string(40, contains_spaces=True),
        "uploaded":True,
        "project":'SITE ' + random_alpha_string(40, contains_spaces=True),
        "isSubmitted":True,
        "isActive":True,
    }
    return inspection

def gen_observation(inspection):
    observation = {
        "inspectionId":inspection['_id'],
        "pinnedAt":random_date_days(-5, -1),
        "_updated_at":random_date_days(-5, -1),
        "_created_at":random_date_days(-20, -5),
        "_uploaded_at":random_date_days(-3, -1),
        "_uploaded_hash":random_alpha_string(40),
        "title":random_alpha_string(40, contains_spaces=True),
        "requirement":random_alpha_string(40, contains_spaces=True),
        "observationDescription":random_alpha_string(40, contains_spaces=True),
    }
    return observation

def gen_audio(observation):
    audio = {
        "inspectionId":observation['inspectionId'],
        "observationId":observation['_id'],
        "_updated_at":random_date_days(-5, -1),
        "_created_at":random_date_days(-20, -5),
        "_uploaded_at":random_date_days(-3, -1),
        "_uploaded_hash":random_alpha_string(40),
        "notes":random_alpha_string(40, contains_spaces=True),
        "index":random.randint(1, 100),
        "title":random_alpha_string(40, contains_spaces=True),
    }
    return audio

def gen_photo(observation):
    photo = {
        "observationId":observation['_id'],
        "inspectionId":observation['inspectionId'],
        "_updated_at":random_date_days(-5, -1),
        "_created_at":random_date_days(-20, -5),
        "_uploaded_at":random_date_days(-3, -1),
        "_uploaded_hash":random_alpha_string(40),
        "caption":random_alpha_string(40, contains_spaces=True),
        "timestamp":random_date_days(-5, -1),
        "index":random.randint(1, 100),
    }
    return photo

def gen_video(observation):
    video = {
        "observationId":observation['_id'],
        "inspectionId":observation['inspectionId'],
        "_updated_at":random_date_days(-5, -1),
        "_created_at":random_date_days(-20, -5),
        "_uploaded_at":random_date_days(-3, -1),
        "_uploaded_hash":random_alpha_string(40),
        "index":random.randint(1, 100),
        "notes":random_alpha_string(40, contains_spaces=True),
        "title":random_alpha_string(40, contains_spaces=True),
    }
    return video


###########################################################################
# gnerate a bunch of sample data in mongodb
###########################################################################

def mongo_sample_data(n_inspections, n_observations):
    mdb_config = config(section='eao_data')
    client = MongoClient('mongodb://%s:%s@%s:%s' % (mdb_config['user'], mdb_config['password'], mdb_config['host'], mdb_config['port']))
    db = client[mdb_config['database']]

    users = db['User']
    teams = db['Team']
    inspections = db['Inspection']
    observations = db['Observation']
    audios = db['Audio']
    photos = db['Photo']
    videos = db['Video']

    user = gen_user()
    user_id = users.insert_one(user).inserted_id
    user = users.find_one({"_id": user_id})

    team = gen_team(user)
    team_id = teams.insert_one(team).inserted_id
    team = teams.find_one({"_id": team_id})

    for i in range(n_inspections):
        inspection = gen_inspection(user, team)
        inspection_id = inspections.insert_one(inspection).inserted_id
        inspection = inspections.find_one({"_id": inspection_id})

        for j in range(n_observations):
            observation = gen_observation(inspection)
            observation_id = observations.insert_one(observation).inserted_id
            observation = observations.find_one({"_id": observation_id})

            audio = gen_audio(observation)
            audio_id = audios.insert_one(audio).inserted_id

            photo = gen_photo(observation)
            photo_id = photos.insert_one(photo).inserted_id

            video = gen_video(observation)
            video_id = videos.insert_one(video).inserted_id

