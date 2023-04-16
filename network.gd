extends Node

const DEFAULT_PORT = 22477
const MAX_CLIENTS = 1

var ip_address = "localhost"
var peer = ENetMultiplayerPeer.new()



func host_server():
	print("Creating server...")
	peer.create_server(DEFAULT_PORT, MAX_CLIENTS)
	#peer.get_host().compress(ENetConnection.COMPRESS_RANGE_CODER)
	multiplayer.multiplayer_peer = peer
	var id = multiplayer.get_unique_id()
	print("Created server. Peer ID: " + str(id))


func join_server():
	print("Joining server...")
	peer.create_client(ip_address, DEFAULT_PORT)
	multiplayer.multiplayer_peer = peer
	var id = multiplayer.get_unique_id()
	print("Joined server. Peer ID: " + str(id))


func reset_network_connection():
	if multiplayer.multiplayer_peer:
		multiplayer.multiplayer_peer.close()
