# Anointed Automation partner portal

Jesus is King ✝️

How to become an Anointed Automation partner, sign in, and find your way around the partner portal.

## Partner types

| Type | What you get |
|---|---|
| **Ads** | You promote our products with your own tracked links and earn on every sale. Portal tabs: Earnings, Links, Ads library, Performance, Payouts |
| **Tech** | You build on our API: product feed, API keys and Sign in with Anointed Automation. Portal tabs: Earnings, Links, Performance, Payouts, Keys, Sign in, Integration |
| **Both** | Everything above |

## Sign up

1. **Apply** at [https://www.anointedautomation.net/partners/apply](https://www.anointedautomation.net/partners/apply).
   Give your name (or business name), your email, your website (optional, https), the type you want and a short
   pitch (up to 1000 characters). One application per email at a time.
2. **Wait for our email.** We read every application ourselves and email you either way. The approval email has
   your sign-in link.
3. **Have an account with the SAME email you applied with.** If you do not have one yet, create it at
   [https://www.anointedautomation.net/signup](https://www.anointedautomation.net/signup) using that email.
4. **Sign in** at [https://www.anointedautomation.net/partner/sign-in](https://www.anointedautomation.net/partner/sign-in)
   with Google, Apple, Microsoft, GitHub or LinkedIn on that email. This page only signs in existing accounts, it
   never creates one. If you use a password, sign in at
   [https://www.anointedautomation.net/login](https://www.anointedautomation.net/login) instead.
5. **Open the portal** at [https://www.anointedautomation.net/partner](https://www.anointedautomation.net/partner).
   The portal finds your partner record from the email you signed in with. If it says your account is not set up
   as a partner, you are signed in with a different email.

## Finding your way around

Everything lives on one page, [https://www.anointedautomation.net/partner](https://www.anointedautomation.net/partner).
Pick a tab at the top. You only see the tabs your partner type includes.

| Tab | Who | What it does |
|---|---|---|
| **Earnings** | Everyone | Your sales, your share and your monthly statements |
| **Links** | Everyone | Build a tracked link to a product, a collection or a store home page |
| **Ads library** | Ads, Both | Products we picked for you to promote, with a ready link and an image |
| **Performance** | Everyone | Clicks, orders, revenue and your commission for any date range |
| **Payouts** | Everyone | Payouts we sent, and your PayPal payout email |
| **Keys** | Tech, Both | Create, revoke and rotate your API keys |
| **Sign in** | Tech, Both | Turn on Sign in with Anointed Automation for a key |
| **Integration** | Tech, Both | Your partner details, your keys at a glance, and your site status if we monitor it |

## For Ads partners

**Tracked links.** On the **Links** tab, choose a product (search by name), a collection, or a store home page
(Anointed, Animal Dander or Mart Club), then copy the link. Every link goes through
`https://api.anointedautomation.net/api/partner/go/<your slug>`, which counts the click and sends the shopper on.
Sales that come through your link are credited to you.

**Ads library.** The **Ads library** tab lists the products we curated for you, best sellers first, with search
and pages. Each product has:

- **Copy link:** your tracked link for that product, ready to paste into a post or ad.
- **Download image:** the product image for your creative.

**Earnings.** The **Earnings** tab shows only your own sales. Figures are net after product cost, fees and tax,
and your share applies your agreed percentage. A sale whose product cost is not recorded yet is left out of your
share until it is, so nothing is estimated. Refunds and lost chargebacks show as negative adjustments.

**Statements.**

- We close each month after a **30 day return window**. A statement for a month is only made once 30 days have
  passed after the month ends, so a refund can never pull back money we already sent you.
- The payout minimum is **$50**. A month under $50 is **carried forward** and added to the next statement. The
  statement shows what was carried in.

**Payouts.** We pay through **PayPal** once a statement is final. The **Payouts** tab lists each payout with its
month, amount, status and reference. "Owed" means it is on its way.

- Your payout email defaults to your API key contact email if you have not set one. With no payout email at all,
  we hold the payout until you add one.
- To change your PayPal payout email, enter the new one on the **Payouts** tab, press **Send me a code**, and
  confirm with the code we email to your account email. We get a heads up too.

## For Tech partners

Keys and sign-in are self-serve. Every change is confirmed with a code we email to your account email (it works
for 15 minutes), and each new key or client secret is shown **once**.

- **Keys tab:** up to **3** active keys. Pick only the permissions you need from `partner-feed:read`,
  `partner:users:provision`, `store-credit:grant` and `partner:self:read`. Revoke a key there too. If a key
  leaks, rotate it on the same tab (optionally keeping the old key working for 24 hours, or rotating only the
  sign-in client secret).
- **Sign in tab:** turn Sign in with Anointed Automation on for a key, with exact https callback URLs (up to
  10), optional after sign out URLs and an optional back-channel logout URL.

Your server trades its key for a short-lived token at `POST https://api.anointedautomation.net/api/auth/machine-token`.
The product feed (`partner-feed:read`) is at:

| Route | What |
|---|---|
| `GET /api/partner/feed/products` | Your curated products, paged |
| `GET /api/partner/feed/products/{handle}` | One product |
| `GET /api/partner/feed/ads` | Ad picks |

Never put a key in a browser, a mobile app or source control.

Full sign-in guide: [Sign in with Anointed Automation](./SIGN_IN_WITH_ANOINTED_AUTOMATION.md).
