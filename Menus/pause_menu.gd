extends CanvasLayer



func _ready():
	#Game.pause_toggled.connect(_on_game_paused_toggle)
	pass
	
	
func _on_new_game_button_pressed():
	visible = false
	#Game.start_new_game()


func _on_game_paused_toggle(is_paused: bool):
	visible = is_paused


func _on_quit_game_button_pressed():
	get_tree().quit()
