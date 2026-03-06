/**
 * Parses the expiry time from a JWT access token without external dependencies.
 * Returns expiry timestamp in milliseconds, or null if the token is invalid.
 */
export function getTokenExpiry(token: string): number | null
{
  try
  {
    const parts = token.split(".");

    if (parts.length !== 3)
    {
      return null;
    }

    // Base64url → base64 → JSON
    const base64 = parts[1]!.replace(/-/g, "+").replace(/_/g, "/");
    const payload = JSON.parse(atob(base64)) as Record<string, unknown>;

    return typeof payload["exp"] === "number" ? payload["exp"] * 1000 : null;
  }
  catch
  {
    return null;
  }
}

/**
 * Returns true if the token is missing, invalid, or will expire within `bufferMs` milliseconds.
 * Default buffer is 60 seconds — refresh before the token actually expires.
 */
export function isTokenExpiringSoon(
  token: string | null,
  bufferMs = 60_000,
): boolean
{
  if (!token)
  {
    return true;
  }

  const exp = getTokenExpiry(token);

  return exp === null || Date.now() >= exp - bufferMs;
}
