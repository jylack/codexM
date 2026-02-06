# Scene Setup Checklist

## RoomScene
- Create a new scene at `Assets/_Project/Scenes/RoomScene.unity`.
- Add exactly one GameObject named `GameBootstrap`.
- Attach `Project.Core.GameBootstrap` component.
- Assign StageDefinitionSO assets to **Stage Definitions**.
- Assign SkillData assets to **Skill Pool**.
- Save scene.

## Login / Room / InGame UI
- No manual Canvas setup is required; `UIRoot` creates Canvas + EventSystem at runtime.
- Login flow:
  - Enter accountId and click Login.
- Room flow:
  - Uses 3-card stage window `[prev,current,next]` with focus on current stage.
  - Click Start Stage.
- InGame flow:
  - Day loop and logs shown.
  - Select one of three skill reward options when offered.
  - Exit Run returns to Room.

## Build Settings
- Add `Assets/_Project/Scenes/RoomScene.unity` to Build Settings as first scene.

## ScriptableObjects
Create assets under:
- `Assets/_Project/ScriptableObjects/Skills`
- `Assets/_Project/ScriptableObjects/Items`
- `Assets/_Project/ScriptableObjects/Stages`
- `Assets/_Project/ScriptableObjects/Monsters`

Minimum stage data to validate loop:
- Stage 1 with at least 2 days.
- Day 1 event = `None`.
- Day 2 event = `Battle` or `CurrencyEvent`.
