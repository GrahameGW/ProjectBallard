extends CheckButton


var game
var piece_cs = load("res://Piece/Piece.cs")
@onready var toggle_value = $"../Visibility/ViewToggle"


func initialize(game_inst):
	self.show()
	self.game = game_inst
	if game.User.Color == 0:
		$UserLabel.text = "White"
		$OpponentLabel.text = "Black"
	else:
		$UserLabel.text = "Black"
		$OpponentLabel.text = "White"


func _on_toggled(_button_pressed):
	game.SwapPlayer()
	game.SwapPieceTokens()
	if toggle_value.selected == 2:
		toggle_value.selected = 1
	elif toggle_value.selected == 1:
		toggle_value.selected = 2
	game.ToggleVisiblePlayer(toggle_value.selected)

