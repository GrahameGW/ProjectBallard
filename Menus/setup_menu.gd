extends CanvasLayer
class_name SetupMenu


@onready var app = self.get_parent()
@onready var sp_tab: Control = $TabContainer/SinglePlayer
@onready var mp_tab: Control = $TabContainer/Multiplayer


func _ready():
	multiplayer.peer_connected.connect(_player_connected)
	multiplayer.peer_disconnected.connect(_player_disconnected)
	multiplayer.server_disconnected.connect(_server_disconnected)


func _on_start_game_pressed_singleplayer():
	var color = sp_tab.get_node("ColorSelect/ColorChoice").selected
	app.StartSingleplayerGame(color)


func _on_start_game_pressed_multiplayer():
	var server_color = mp_tab.get_node("ColorSelect/ColorChoice").selected
	app.start_multiplayer_game.rpc(server_color)


func _on_host_server_pressed():
	Network.host_server()
	mp_tab.get_node("IpAddress").editable = false
	mp_tab.get_node("JoinServer").hide()
	mp_tab.get_node("HostServer").hide()
	mp_tab.get_node("CloseServer").show()
	mp_tab.get_node("ColorSelect").show()
	
	var start = mp_tab.get_node("StartGame")
	start.show()
	start.disabled = true

	var status = mp_tab.get_node("ConnStatus")
	status.text = "Waiting for opponent..."
	status.show()


func _on_close_server_pressed():
	Network.reset_network_connection()
	mp_tab.get_node("IpAddress").editable = true
	mp_tab.get_node("JoinServer").show()
	mp_tab.get_node("HostServer").show()
	mp_tab.get_node("CloseServer").hide()
	mp_tab.get_node("ConnStatus").hide()
	mp_tab.get_node("ColorSelect").hide()
	mp_tab.get_node("StartGame").hide()


func _on_join_server_pressed():
	Network.join_server()
	mp_tab.get_node("IpAddress").editable = false
	mp_tab.get_node("JoinServer").hide()
	mp_tab.get_node("HostServer").hide()
	mp_tab.get_node("LeaveServer").show()
	
	var status = mp_tab.get_node("ConnStatus")
	status.text = "Waiting for host to start the game..."
	status.show()


func _on_leave_server_pressed():
	Network.reset_network_connection()
	mp_tab.get_node("IpAddress").editable = true
	mp_tab.get_node("JoinServer").show()
	mp_tab.get_node("HostServer").show()
	mp_tab.get_node("LeaveServer").hide()
	mp_tab.get_node("ConnStatus").hide()


func _on_ip_address_changed(address: String):
	Network.ip_address = address


func _player_connected(_id):
	if multiplayer.is_server():
		mp_tab.get_node("StartGame").disabled = false
		mp_tab.get_node("ConnStatus").text = "Player joined. Ready to start?"


func _player_disconnected(_id):
	app.close_multiplayer_game()
	mp_tab.get_node("IpAddress").editable = false
	mp_tab.get_node("JoinServer").hide()
	mp_tab.get_node("HostServer").hide()
	mp_tab.get_node("CloseServer").show()
	mp_tab.get_node("ColorSelect").hide()
	mp_tab.get_node("ConnStatus").text = "Player left. Waiting for opponent..."
	
	var start = mp_tab.get_node("StartGame")
	start.show()
	start.disabled = true


func _server_disconnected():
	app.close_multiplayer_game()
	mp_tab.get_node("IpAddress").editable = true
	mp_tab.get_node("JoinServer").show()
	mp_tab.get_node("HostServer").show()
	mp_tab.get_node("CloseServer").hide()
	mp_tab.get_node("ConnStatus").hide()
	mp_tab.get_node("ColorSelect").hide()
	mp_tab.get_node("StartGame").hide()
