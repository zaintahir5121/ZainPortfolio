# Lead capture — setup

The static site has no backend, so the three forms post to a hosted form
service. **Until you paste a key below, nothing is sent** — the forms say so
plainly and offer a direct email link instead. They never claim a message was
delivered when it was not.

## 1. Get a key (2 minutes, free)

1. Go to **https://web3forms.com**
2. Enter the inbox you want leads delivered to
3. Copy the **Access Key** they email you

No account, no password, no card. The free tier covers 250 submissions a
month, which is well beyond what a portfolio generates.

## 2. Paste it in

`lead.js`, near the top:

```js
var CONFIG = {
  accessKey: 'paste-your-key-here',
  ...
};
```

That is the only change. Reload the page and submit a test from the contact
form — it should land in your inbox within a few seconds.

## What gets captured

| Form | Where | What arrives |
|---|---|---|
| **Spend calculator** | Approach section | Email **plus their own slider values** — queries/day, tokens, teams, and the before/after figures they were looking at |
| **Contact form** | Contact section | Name, email, subject, message |
| **Newsletter** | Footer | Email and which form it came from |

The calculator capture is the one worth watching. By the time someone asks for
that breakdown they have already spent half a minute thinking about their own
AI spend, and the submission tells you the size of the problem before you
reply. A lead that says *300,000 queries/day across 8 teams* is a different
conversation from one that says *1,000*.

## Spam

Every form carries a honeypot: a hidden `botcheck` field that real people
never see. Submissions that fill it are dropped silently — the bot gets a
normal-looking response and you get nothing in your inbox.

## If you later move to the .NET CMS

`src/ZainPortfolio` already has a `Subscriber` table, a `/subscribe` endpoint
with unsubscribe tokens, and an admin dashboard. To switch, point
`CONFIG.endpoint` at `/subscribe` and drop `access_key` from the payload —
the shape of the rest already matches.

## Changing the wording

All visitor-facing text lives in `i18n.js` under the `ld.*` keys, in English,
German and Malay.
