/**
 * Whitelist-based HTML sanitizer (CWE-116 compliant)
 * Replaces incomplete multi-character sanitization with token-based validation
 */

const ALLOWED_TAGS = new Set([
  'p', 'br', 'strong', 'b', 'em', 'i', 'span', 'div',
  'h1', 'h2', 'h3', 'blockquote', 'ul', 'ol', 'li', 'a'
]);

const ALLOWED_ATTRIBUTES = {
  'a': ['href', 'title'],
  'div': ['class'],
  'span': ['class']
};

function sanitizeUrl(url) {
  if (!url || typeof url !== 'string') return '';
  const dangerous = /^(javascript|data|vbscript|file):/i;
  if (dangerous.test(url)) return '';
  return url;
}

function sanitizeHtml(html) {
  if (!html) return '';
  let result = '';
  const tagRegex = /<\/?([a-z][a-z0-9]*)\b([^>]*)>/gi;
  let lastIndex = 0;
  let match;

  while ((match = tagRegex.exec(html)) !== null) {
    // Add escaped text before tag
    result += html.substring(lastIndex, match.index)
      .replace(/</g, '&lt;').replace(/>/g, '&gt;');

    const tagName = match[1].toLowerCase();
    const isClosing = match[0].startsWith('</');

    if (!ALLOWED_TAGS.has(tagName)) {
      lastIndex = tagRegex.lastIndex;
      continue; // Skip disallowed tags
    }

    if (isClosing) {
      result += `</${tagName}>`;
    } else {
      // Build clean opening tag
      let cleanTag = `<${tagName}`;
      const allowed = ALLOWED_ATTRIBUTES[tagName] || [];
      
      // Extract and validate attributes
      const attrMatches = match[2].matchAll(/(\w+)\s*=\s*["']([^"']*)["']/g);
      for (const attr of attrMatches) {
        const [, name, value] = attr;
        if (allowed.includes(name.toLowerCase())) {
          const cleanValue = name.toLowerCase() === 'href' 
            ? sanitizeUrl(value)
            : value.replace(/"/g, '&quot;');
          if (cleanValue) cleanTag += ` ${name}="${cleanValue}"`;
        }
      }
      
      cleanTag += ['br', 'img'].includes(tagName) ? ' />' : '>';
      result += cleanTag;
    }
    lastIndex = tagRegex.lastIndex;
  }

  // Add remaining text
  result += html.substring(lastIndex)
    .replace(/</g, '&lt;').replace(/>/g, '&gt;');

  return result;
}

module.exports = { sanitizeHtml, sanitizeUrl };