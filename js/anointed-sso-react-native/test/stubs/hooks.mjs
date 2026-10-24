// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Resolve hook: maps "react-native" to the stub so the real button module can be imported under node.
export async function resolve(specifier, context, next) {
  if (specifier === "react-native") return { url: new URL("./react-native.js", import.meta.url).href, shortCircuit: true };
  return next(specifier, context);
}
