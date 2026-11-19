# Portfolio images

These five files are referenced by the Milestones & Recognition section in
`index.html`. They are stored here rather than hotlinked from LinkedIn,
because LinkedIn CDN URLs carry an expiry token (`?e=...`) and stop resolving
after a few days.

## Save the five photos here

Use these exact filenames — the page looks for them by name.

| Filename | Photo |
|---|---|
| `mtbc-team.jpg` | The large formal group photo — MTBC (CareCloud), Islamabad |
| `dhl-team-kl.jpg` | The group in the shopping mall, dark polo shirts |
| `award-ceremony.jpg` | Receiving the award in academic robes |
| `certificate-presentation.jpg` | Receiving the certificate in the auditorium |
| `team-celebration.jpg` | The three of you at the event |

Lower case, `.jpg`, no spaces.

## If a file is missing

The tile degrades to a labelled dashed placeholder instead of a broken-image
icon, so the page stays presentable while you collect them.

## Captions

Captions live in `i18n.js` under the keys `ac.g1` … `ac.g5`, in all three
languages. Change the text there if a caption is wrong; nothing else needs
touching.

## Size

Keep each file under roughly 400 KB. squoosh.app will do it with no visible
loss. Large photos are the single easiest way to make a fast page feel slow.
