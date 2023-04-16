extends CanvasLayer


@onready var app:= get_parent()
@onready var result_display := find_child("ResultLabel")


func _on_quit_game_pressed():
	get_tree().quit()


func _on_rematch_same_color_pressed():
	#var server_color = get_server_color()
	#app.start_multiplayer_game.rpc(server_color)
	pass


func _on_rematch_swap_color_pressed():
	#var server_color = get_server_color()
	#app.start_multiplayer_game.rpc(server_color^1)
	pass


func get_server_color():
	var children = app.game_instance.get_children()
	#var players = children.filter(func(c): return layer)
	#for player in players:
	#	if player.peer_id == 1:
	#		return player.color


func display_result(result: int):
	if result == 0:
		result_display.text = "White wins!"
	elif result == 1:
		result_display.text = "Black wins!"
	else:
		result_display.text = "Drawn game"
