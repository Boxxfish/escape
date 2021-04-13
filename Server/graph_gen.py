"""
Puzzle graph generator.
Can be used in isolation to output puzzle graphs using the terminal.
"""

from pathlib import Path
import random
import yaml
import networkx as nx
from matplotlib import pyplot as plt

# Stores and queries facts about puzzles.
class KnowledgeBase:

    # When initialized, load all info from definitions folder.
    def __init__(self, defs_path):
        d_path = Path(defs_path)
        i_path = d_path / "items"
        i_traits_path = d_path / "item_traits"
        
        self.items = {}
        for i_file in i_path.iterdir():
            id = i_file.stem
            item_data = yaml.load(i_file.open(), Loader=yaml.CLoader)
            self.items[id] = item_data
        
        self.i_traits = {}
        for i_traits_file in i_traits_path.iterdir():
            id = i_traits_file.stem
            i_traits_data = yaml.load(i_traits_file.open(), Loader=yaml.CLoader)
            self.i_traits[id] = i_traits_data
            
    # Returns True if an item trait is a supertrait of another.
    # Also returns True if the traits are the same.
    def is_item_super(self, supertrait, trait):
        curr_trait = trait
        while curr_trait is not None:
            if curr_trait == supertrait:
                return True
            curr_trait = self.i_traits[curr_trait]["from"]
        return False
        
    # Returns True if a set of traits are all supertraits of another.
    # Alos returns True if the traits are the same.
    def is_item_super_multi(self, supertraits, traits):
        for supertrait in supertraits:
            is_super = False
            for trait in traits:
                if self.is_item_super(supertrait, trait):
                    is_super = True
                    break
            if not is_super:
                return False
        return True
            
    # Returns True if a usage can return an item with specific traits.
    def usage_can_return_item(self, item_id, usage_id, traits):
        usage_outputs = self.items[item_id]["usage"][usage_id]["out"]
        for usage_output in usage_outputs:
            if usage_output["type"] == "item" and self.is_item_super_multi(usage_output["traits"], traits):
                return True
        return False
            
    # Returns True if an item can return another that has specific traits.
    def item_can_return_item(self, item_id, traits):
        # Get the item's usages 
        usages = self.items[item_id]["usage"]
        if usages is None:
            return False
        # Check if one of the usages returns an item with the required traits
        for usage in usages:
            usage_outputs = self.items[item_id]["usage"][usage]["out"]
            for usage_output in usage_outputs:
                if usage_output["type"] != "item":
                    continue
                output_traits = usage_output["traits"]
                if self.is_item_super_multi(output_traits, traits):
                    return True
        return False
            
    # Returns a set of item IDs that can return an item with specific traits.
    def can_return_item(self, traits):
        items = []
        for item in self.items:
            if self.item_can_return_item(item, traits):
                items.append(item)
        return items
            
    # Returns True if a usage can perform an action.
    def usage_can_perform_action(self, item_id, usage_id, action_id):
        usage_outputs = self.items[item_id]["usage"][usage_id]["out"]
        for usage_output in usage_outputs:
            if usage_output["type"] == "action" and usage_output["id"] == action_id:
                return True
        return False
            
    # Returns True if an item can perform an action.
    def item_can_perform_action(self, item_id, action_id):
        # Get the item's usages 
        usages = self.items[item_id]["usage"]
        if usages is None:
            return False
        # Check if one of the usages peforms an action with the action id
        for usage in usages:
            usage_outputs = self.items[item_id]["usage"][usage]["out"]
            for usage_output in usage_outputs:
                if usage_output["type"] != "action":
                    continue
                if usage_output["id"] == action_id:
                    return True
        return False
            
    # Returns a set of item IDs that can perform an action.
    def can_perform_action(self, action_id):
        items = []
        for item in self.items:
            if self.item_can_perform_action(item, action_id):
                items.append(item)
        return items
        
    # Returns all usage IDs an item has.
    def get_item_usages(self, item_id):
        i_data = self.items[item_id]
        if i_data["usage"] is None:
            return []
        return [usage for usage in i_data["usage"]]
        
    # Returns all usage IDs an item has that have prerequisites.
    def get_item_usages_w_prereqs(self, item_id):
        i_data = self.items[item_id]
        if i_data["usage"] is None:
            return []
        return [usage for usage in i_data["usage"] if i_data["usage"][usage]["in"] is not None]
        
    # Returns all usage IDs an item has that have no prerequisites.
    def get_item_usages_no_prereqs(self, item_id):
        i_data = self.items[item_id]
        if i_data["usage"] is None:
            return []
        return [usage for usage in i_data["usage"] if i_data["usage"][usage]["in"] is None]
        
# Generates a puzzle graph.
# kb: Knowledge base.
# min_n: Minimum number of nodes to create.
def create_graph(kb, min_n):
    n_created = 0
    graph = nx.DiGraph()
    node_queue = []
    
    # Create initial node (door) and add to queue
    node = {
        "id": "door",
        "name": "door" + str(n_created),
        "usage": random.choice(kb.get_item_usages_w_prereqs("door"))
    }
    node_queue.append(node)
    graph.add_node(node["name"])
    
    # Expand nodes.
    # If n_created > min_n, stop looking for items with prerequisites.
    while len(node_queue) > 0:
        node = node_queue.pop(0)        
        if node["id"] == "room":
            continue
        # If usage is None, create a node that spawns this node.
        # Usually, this is just a "room" node.
        if node["usage"] is None or kb.items[node["id"]]["usage"][node["usage"]]["in"] is None:
            next_item = random.choice(kb.can_return_item(kb.items[node["id"]]["traits"]))
            next_usage_choices = None
            if n_created <= min_n:
                next_usage_choices = kb.get_item_usages_w_prereqs(next_item)
            else:
                next_usage_choices = kb.get_item_usages_no_prereqs(next_item)
            next_usage = None
            if len(next_usage_choices) > 0:
                next_usage = random.choice(next_usage_choices)
            n_created += 1
            new_node = {
                "id": next_item,
                "name": next_item + str(n_created),
                "usage": next_usage
            }
            node_queue.append(new_node)
            graph.add_node(new_node["name"])
            graph.add_edge(new_node["name"], node["name"])
        else:
            prereqs = kb.items[node["id"]]["usage"][node["usage"]]["in"]
            for prereq in prereqs:
                # Choose the next item to add.
                # There should always be an item to add, since the 
                # "room" item gives the player items unconditionally.
                next_item_choices = []
                if prereq["type"] == "item":
                    next_item_choices = kb.can_return_item(prereq["traits"])
                elif prereq["type"] == "action":
                    next_item_choices = kb.can_perform_action(prereq["id"])
                
                # Create a new node and add to queue
                next_item = random.choice(next_item_choices)
                next_usage_choices = None
                if n_created <= min_n:
                    next_usage_choices = kb.get_item_usages_w_prereqs(next_item)
                else:
                    next_usage_choices = kb.get_item_usages_no_prereqs(next_item)
                
                if prereq["type"] == "item":
                    next_usage_choices = [x for x in next_usage_choices if kb.usage_can_return_item(next_item, x, prereq["traits"])]
                elif prereq["type"] == "action":
                    next_usage_choices = [x for x in next_usage_choices if kb.usage_can_perform_action(next_item, x, prereq["id"])]
                
                next_usage = None
                if len(next_usage_choices) > 0:
                    next_usage = random.choice(next_usage_choices)
                n_created += 1
                new_node = {
                    "id": next_item,
                    "name": next_item + str(n_created),
                    "usage": next_usage
                }
                node_queue.append(new_node)
                graph.add_node(new_node["name"])
                graph.add_edge(new_node["name"], node["name"])
            
    
    return graph
    
if __name__ == "__main__":
    kb = KnowledgeBase("defs")
    
    # Generate and show graph
    graph = create_graph(kb, 20)
    nx.draw_networkx(graph)
    plt.show()