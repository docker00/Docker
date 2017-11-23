#!/usr/bin/env python3

import pdb
import flask
#import logging
import json
#from flask_oidc import discovery
from flask_oidc import OpenIDConnect
#from flask.ext.oidc import OpenIDConnect

from flask_cors import CORS

#logging.basicConfig()

app = flask.Flask(__name__)
CORS(app)

app.config.update({
    'SECRET_KEY': 'secret',
    'TESTING': True,
    'DEBUG': True,
    'OIDC_CLIENT_SECRETS': 'client_secrets.json',
    'OIDC_ID_TOKEN_COOKIE_SECURE': False,
    'OIDC_REQUIRE_VERIFIED_EMAIL': False,
    'OIDC_VALID_ISSUERS': ['http://localhost:5000','http://rum-zakharov.scanex.ru:5000'],
    'OIDC_SCOPES': 'openid email profile',
    #'OVERWRITE_REDIRECT_URI': 'http://192.168.0.212:8080/python/oidc_callback'
})

oidc = OpenIDConnect(app)
#OP = discovery.discover_OP_information("http://192.168.0.212:5000/.well-known/openid-configuration")

# disables JSON pretty-printing in flask.jsonify
# app.config['JSONIFY_PRETTYPRINT_REGULAR'] = False


def to_json(data):
    return json.dumps(data) + "\n"


def resp(code, data):
    return flask.Response(
        status=code,
        mimetype="application/json",
        response=to_json(data)
    )


def theme_validate():
    errors = []
    json = flask.request.get_json()
    if json is None:
        errors.append(
            "No JSON sent. Did you forget to set Content-Type header" +
            " to application/json?")
        return (None, errors)

    for field_name in ['title', 'url']:
        if type(json.get(field_name)) is not str:
            errors.append(
                "Field '{}' is missing or is not a string".format(
          field_name))

    return (json, errors)


def affected_num_to_code(cnt):
    code = 200
    if cnt == 0:
        code = 404
    return code


@app.route('/')
#@oidc.require_login
def root():
    #return resp(200, {"theme_id": "theme_id"})
    return flask.redirect('/api/1.0/themes')

# e.g. failed to parse json
@app.errorhandler(400)
def page_not_found(e):
    return resp(400, {})


@app.errorhandler(404)
def page_not_found(e):
    return resp(400, {})


@app.errorhandler(405)
def page_not_found(e):
    return resp(405, {})


@app.route('/api/1.0/themes', methods=['GET'])
@oidc.require_login
def get_themes():
    themes = [1,2,3,4,5]
    return resp(200, {"themes": themes})

@app.route('/api/1.0/themes2', methods=['GET'])
@oidc.accept_token(True, ['openid'])
def get_themes2():
    themes = [1,2,3,4,5]
    return resp(200, {"themes": themes})

@app.route('/api/1.0/themes', methods=['POST'])
@oidc.require_login
def post_theme():
    (json, errors) = theme_validate()
    if errors:  # list is not empty
        return resp(400, {"errors": errors})

    with db_conn() as db:
        insert = db.prepare(
            "INSERT INTO themes (title, url) VALUES ($1, $2) " +
            "RETURNING id")
        [(theme_id,)] = insert(json['title'], json['url'])
        return resp(200, {"theme_id": theme_id})


@app.route('/api/1.0/themes/<int:theme_id>', methods=['PUT'])
@oidc.require_login
def put_theme(theme_id):
    (json, errors) = theme_validate()
    if errors:  # list is not empty
        return resp(400, {"errors": errors})

    with db_conn() as db:
        update = db.prepare(
            "UPDATE themes SET title = $2, url = $3 WHERE id = $1")
        (_, cnt) = update(theme_id, json['title'], json['url'])
        return resp(affected_num_to_code(cnt), {})


@app.route('/api/1.0/themes/<int:theme_id>', methods=['DELETE'])
@oidc.require_login
def delete_theme(theme_id):
    with db_conn() as db:
        delete = db.prepare("DELETE FROM themes WHERE id = $1")
        (_, cnt) = delete(theme_id)
        return resp(affected_num_to_code(cnt), {})

#@app.before_request
#@oidc.require_login
#def before_request():
#    issuer = client.discover('ronkajitsu@mail.ru')
#    provider_info = client.provider_config(issuer)
#    provider_info["authorization_endpoint"]
#    if 'logged_in' not in session and request.endpoint != 'login':
#        return redirect(url_for('login'))

if __name__ == '__main__':
    app.debug = True  # enables auto reload during development
    app.run(host='0.0.0.0', port=5001)

