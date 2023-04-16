extends VBoxContainer

var game


func initialize(game_inst):
	self.show()
	self.game = game_inst

func _on_view_toggle_item_selected(index):
	game.ToggleVisiblePlayer(index)
