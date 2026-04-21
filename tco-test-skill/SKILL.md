---
name: tco-test-skill
description: Use when adding or modifying TCO-related test templates in this LCD test system, especially when the work involves template types, coordinate generation/editing, TCO angle measurement flow, and coordinate compensation that depends on equipment mechanism type.
---

# TCO Test Skill

This skill is for the `LCD` WPF project in this repo.

Use it when the task is to:

- add a new TCO test type into the template system
- make selecting TCO enter the coordinate generation/editing flow
- connect coordinate calculation to equipment mechanism type
- align implementation with the local `TCO测量方法(1).docx`

Read [references/code-map.md](references/code-map.md) first.
Read [references/tco-method-summary.md](references/tco-method-summary.md) when the task depends on TCO measurement rules.

## Core understanding

TCO in this codebase should be treated as a new `ENUMMESSTYLE`-level capability, not just a template name.

Why:

- `Template.xml` stores template instances, but behavior is driven by `MESTYPE`
- template creation/editing UI branches by `MESTYPE`
- execution and coordinate compensation also branch by `MESTYPE`

## Existing execution model

The current system already has a strong split:

1. `Project.lstInfos` persists template definitions.
2. `CustomView` creates, lists, selects, and opens template editors.
3. `CustomTemplate` edits point-like template tables.
4. `DataTemplateView` displays the selected template table.
5. `ProcessCtrl` executes selected templates.
6. `PointF.UpdateByAlgorithm1()` applies mechanism-dependent coordinate compensation.

For point-like templates, coordinate compensation is already tied to equipment mechanism:

- `ProcessCtrl.ProcessPointTemplateXYZ()` reads `Project.cfg.EQType`
- `EquipmentType.Type_D` uses direct motion
- other types call `PointF.UpdateByAlgorithm1(height, Project.PtCenter)`
- `PointF.UpdateByAlgorithm1()` switches on `EquipmentType.Type_A/B/C/E`

So for TCO, prefer reusing the existing XYZ path and adding TCO-specific coordinate generation rules, instead of creating a second compensation engine.

## Recommended implementation path

### 1. Add a new test type

Add a new enum member for TCO and update all places that map enum <-> UI text.

Minimum checkpoints:

- `LCD/Data/Project.cs`: `ENUMMESSTYLE`
- `LCD/View/CustomView.xaml`: template type combo box
- `LCD/View/CustomView.xaml.cs`: `comboBoxType_SelectionChanged()`
- `LCD/View/CustomView.xaml.cs`: `OnBnClickedAddTemplate()`
- `LCD/View/CustomView.xaml.cs`: `update_type()`
- `LCD/View/DataTemplateView.xaml.cs`: table init switch
- `LCD/Ctrl/ProcessCtrl.cs`: `Run()` switch

### 2. Make TCO enter coordinate generation/editing

Current entry is:

- `CustomView.OnBnClickedAutoCreate()`
- then `new CustomTemplate(info, infoData)`

If TCO needs a dedicated coordinate generator/editor, add a branch here:

- `if (info.MESTYPE == ENUMMESSTYLE.TCO) open new TCO editor`
- else keep existing `CustomTemplate`

If the current `CustomTemplate` can be extended cleanly, reuse it and inject TCO-specific generation rules.

### 3. Keep execution on the XYZ motion path

TCO angle testing is still fundamentally a point-template measurement with U/V angle changes and point position changes.

Prefer:

- TCO template table columns remain compatible with point templates
- generate rows containing `X/Y/Z/U/V/PG`
- execute via `ProcessPointTemplateXYZ()`

This keeps:

- motion safety checks
- device movement
- PG switching
- result collection
- equipment compensation

all on existing, tested paths.

### 4. Put TCO-specific logic in coordinate generation, not in motion execution

TCO differs mainly in:

- which points are measured
- which angle combinations are required
- how those rows are generated from product geometry and TCO rules

That logic belongs in the template editor/generator stage.

Suggested separation:

- editor/generator: build TCO measurement rows
- executor: reuse `ProcessPointTemplateXYZ()`
- evaluator/reporting: compute TCO ratios and pass/fail

### 5. Bind generation to equipment mechanism inputs

When generating TCO coordinates, always consider:

- `Project.cfg.EQType`
- `Project.PtCenter`
- `Project.Xorg/Yorg/Zorg/Uorg/Vorg`
- template `height`
- axis compensation direction such as `Project.cfg.ax_z.CompensationDirection`

Important:

- the same nominal TCO angle target may need different actual XYZ compensation depending on mechanism type
- therefore "TCO coordinate generation" should not be purely geometric screen math; it must stay compatible with the existing mechanism compensation model

## Practical guidance for TCO

Based on the local TCO method, the software likely needs:

- 5 measurement positions on screen
- horizontal rotation cases around the display center: `+30`, `-30`
- vertical tilt cases around the display center: `+15`, `-15`
- point labels that distinguish left/right or top/bottom positions per angle set

Likely template strategy:

1. Generate base 5 screen points.
2. Expand them into TCO-required angle groups.
3. Store angle commands in `U/V` per row.
4. Keep execution through existing XYZ logic.
5. Add TCO-specific evaluation after measurement if the output must report TCO ratios.

## When implementing

Avoid these traps:

- Do not only add `"TCO"` as a template name.
- Do not bypass `MESTYPE`; behavior will not propagate.
- Do not hardcode one compensation formula in the TCO editor.
- Do not duplicate `ProcessPointTemplateXYZ()` unless TCO truly needs a different runtime path.

Prefer these checks:

- confirm whether TCO maps closer to `_01_POINT` semantics or deserves its own enum
- confirm whether `CustomTemplate` can host TCO generation controls
- confirm whether result evaluation needs new fields or only post-processing

