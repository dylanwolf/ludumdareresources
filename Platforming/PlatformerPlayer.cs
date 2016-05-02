using UnityEngine;
using System.Collections;

public class PlatformerPlayer : PlatformerCharacter {

	const string INPUT_HORIZONTAL = "Horizontal";
	const string INPUT_JUMP = "Jump";
	InputResponse tmpInput = new InputResponse();

	protected override InputResponse? GetInput()
	{
		tmpInput.HorizontalDirection = Input.GetAxisRaw(INPUT_HORIZONTAL);
		tmpInput.Jump = Input.GetButton(INPUT_JUMP);

		return tmpInput;
    }

}
