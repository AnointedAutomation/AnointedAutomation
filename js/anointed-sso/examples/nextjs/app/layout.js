// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

export const metadata = { title: "Anointed SSO Next.js example" };

/**
 * @summary    Root layout.
 * @param {{children: React.ReactNode}} props
 * @returns {JSX.Element}
 */
export default function RootLayout({ children }) {
  return (
    <html lang="en">
      <body>{children}</body>
    </html>
  );
}
