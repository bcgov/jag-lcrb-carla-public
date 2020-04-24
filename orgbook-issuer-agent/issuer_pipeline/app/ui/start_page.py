import flask

blueprint = flask.Blueprint('start_page', __name__)


@blueprint.route('/')
def start_page():
    return flask.redirect(flask.url_for('data_integration.node_page'))
