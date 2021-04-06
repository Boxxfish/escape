"""
Central server for Escape.io.
"""

from flask import Flask, jsonify, request, render_template
import random
import string
app = Flask(__name__)

rooms = {}

@app.route("/")
def home():
    return render_template("index.html")
    
@app.route("/genpuzzles")
def gen_puzzles():
    return jsonify({
        "status": "ok",
        "items": []
    })

@app.route("/createroom")
def create_room():
    player_name = request.args.get("name")
    room_data = {
        "players": [player_name],
        "host": player_name
    }
    room_code = "".join(random.choice(string.ascii_uppercase + string.digits) for _ in range(6))
    rooms[room_code] = room_data
    return jsonify({
        "status": "ok",
        "code": room_code
    })
    
@app.route("/joinroom")
def join_room():
    player_name = request.args.get("name")
    return jsonify({
        "status": "ok"
    })

if __name__ == "__main__":
    app.run(debug=True)
