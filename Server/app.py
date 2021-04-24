"""
Central server for Escape.io.
"""

from flask import Flask, jsonify, request, render_template
from graph_gen import KnowledgeBase, create_graph
import random
import string

# Globals
app = Flask(__name__)
kb = KnowledgeBase("defs")

rooms = {}

@app.route("/")
def home():
    return render_template("index.html")
    
@app.route("/genpuzzles")
def gen_puzzles():
    # Generate puzzle graph
    root_name, items, _ = create_graph(kb, 10)
    
    return jsonify({
        "status": "ok",
        "root_name": root_name,
        "items": items,
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
