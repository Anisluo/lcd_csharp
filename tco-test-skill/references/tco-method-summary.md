# TCO Method Summary

Source:

- local file `TCO测量方法(1).docx`

Section observed:

- `5.10 亮度均匀性-角度依赖性`

## Key requirements extracted

- Measure luminance at 5 different screen positions.
- Measurement area is white `RGB255,255,255`.
- White area size is `4%` of active screen size.
- Background is `RGB102,102,102`.
- The luminance meter points toward the screen center.
- All measurements are taken through a fixed observation point.
- Measurement distance is `1.5 x screen diagonal`, but not less than `500 mm`.

## Angle cases

Horizontal direction:

- rotate around the vertical axis through the display glass center
- `+30°`
- `-30°`

Vertical direction:

- rotate around the horizontal axis through the display glass center
- `+15°` backward
- `-15°` forward

## Evaluation logic

Horizontal direction:

- compare the two measured luminance values for each horizontal angle case
- convert to `Lmax/Lmin`
- take the average of the `+30°` and `-30°` ratios
- target requirement shown in the document: `<= 1.73`

Vertical direction:

- compare the two measured luminance values for each vertical angle case
- convert to `Lmax/Lmin`
- take the larger value of the `+15°` and `-15°` ratios
- target requirement shown in the document: `<= 1.73`

## Implication for software design

TCO is not just a normal point template:

- it needs structured angle groups
- it needs fixed position semantics like left/right or top/bottom
- it benefits from auto-generated coordinate rows
- it may need dedicated result evaluation beyond plain point logging

## Recommended software shape

- use a dedicated TCO template type
- keep row storage compatible with point tables
- auto-generate rows for the required positions and angle cases
- execute using the existing XYZ compensation path
- add TCO-specific evaluation/reporting after measurement
