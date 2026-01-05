/**
 * @file Url_Extractor.js
 * @author ICAZA <github.wildly512@passinbox.com>
 * @version 1.1
 * @description Extract URL from a web page.
 * @license MIT
 */

(function() {
  'use strict';

  if (window.__urlExtractorLoaded) return;
  window.__urlExtractorLoaded = true;

  const escapeHtml = str => {
    const div = document.createElement('div');
    div.textContent = str;
    return div.innerHTML;
  };

  const extractUrls = () => {
    const links = Array.from(document.querySelectorAll('a[href]'));
    const urlMap = new Map();

    links.forEach(link => {
      const href = link.href;
      if (!href || href.startsWith('javascript:')) return;

      const text = link.textContent.trim() || link.title || href;
      const domain = new URL(href).hostname;

      if (!urlMap.has(href)) {
        urlMap.set(href, {
          url: href,
          text: text,
          domain: domain,
          count: 1
        });
      } else {
        urlMap.get(href).count++;
      }
    });

    return Array.from(urlMap.values()).sort((a, b) => 
      a.domain.localeCompare(b.domain) || a.text.localeCompare(b.text)
    );
  };

  const groupByDomain = urls => {
    const grouped = {};
    urls.forEach(item => {
      if (!grouped[item.domain]) grouped[item.domain] = [];
      grouped[item.domain].push(item);
    });
    return grouped;
  };

  const exportMarkdown = urls => {
    let md = `# URLs extraites de ${escapeHtml(document.title)}\n\n`;
    md += `**Date:** ${new Date().toLocaleString()}\n`;
    md += `**Total:** ${urls.length} liens uniques\n\n---\n\n`;

    const grouped = groupByDomain(urls);
    Object.keys(grouped).sort().forEach(domain => {
      md += `## ${domain}\n\n`;
      grouped[domain].forEach(item => {
        md += `- [${item.text}](${item.url})`;
        if (item.count > 1) md += ` *(${item.count}×)*`;
        md += '\n';
      });
      md += '\n';
    });

    return md;
  };

  const exportCsv = urls => {
    const escape = str => `"${String(str).replace(/"/g, '""')}"`;
    let csv = 'URL,Texte,Domaine,Occurrences\n';
    urls.forEach(item => {
      csv += `${escape(item.url)},${escape(item.text)},${escape(item.domain)},${item.count}\n`;
    });
    return csv;
  };

  const download = (content, filename, type) => {
    const blob = new Blob([content], { type });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = filename;
    a.click();
    URL.revokeObjectURL(url);
  };

  const createUI = urls => {
    const container = document.createElement('div');
    container.id = 'url-extractor-panel';
    container.style.cssText = `
      position: fixed; top: 20px; right: 20px; width: 400px; max-height: 80vh;
      background: #fff; border: 2px solid #333; border-radius: 8px;
      box-shadow: 0 4px 20px rgba(0,0,0,0.3); z-index: 999999;
      font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif;
      display: flex; flex-direction: column; overflow: hidden;
    `;

    const header = `
      <div style="padding: 15px; background: #333; color: #fff; display: flex; justify-content: space-between; align-items: center;">
        <strong style="font-size: 16px;">📎 URLs Extraites (${urls.length})</strong>
        <button id="close-extractor" style="background: #f44336; color: #fff; border: none; padding: 5px 10px; border-radius: 4px; cursor: pointer; font-weight: bold;">✕</button>
      </div>
    `;

    const buttons = `
      <div style="padding: 10px; background: #f5f5f5; display: flex; gap: 8px; border-bottom: 1px solid #ddd;">
        <button id="export-md" style="flex: 1; padding: 8px; background: #4CAF50; color: #fff; border: none; border-radius: 4px; cursor: pointer; font-weight: bold;">📄 MD</button>
        <button id="export-csv" style="flex: 1; padding: 8px; background: #2196F3; color: #fff; border: none; border-radius: 4px; cursor: pointer; font-weight: bold;">📊 CSV</button>
      </div>
    `;

    const grouped = groupByDomain(urls);
    let listHtml = '<div style="overflow-y: auto; padding: 15px; flex: 1;">';
    
    Object.keys(grouped).sort().forEach(domain => {
      listHtml += `<div style="margin-bottom: 20px;">`;
      listHtml += `<h3 style="margin: 0 0 10px 0; color: #333; font-size: 14px; border-bottom: 2px solid #4CAF50; padding-bottom: 5px;">🌐 ${escapeHtml(domain)}</h3>`;
      
      grouped[domain].forEach(item => {
        const displayText = item.text.length > 60 ? item.text.substring(0, 60) + '...' : item.text;
        listHtml += `
          <div style="margin-bottom: 8px; padding: 8px; background: #f9f9f9; border-radius: 4px; border-left: 3px solid #4CAF50;">
            <a href="${escapeHtml(item.url)}" target="_blank" rel="noopener noreferrer" style="color: #1976D2; text-decoration: none; font-size: 13px; word-break: break-all; display: block;">
              ${escapeHtml(displayText)}
            </a>
            ${item.count > 1 ? `<span style="color: #666; font-size: 11px; margin-top: 4px; display: block;">Répété ${item.count} fois</span>` : ''}
          </div>
        `;
      });
      listHtml += '</div>';
    });
    listHtml += '</div>';

    container.innerHTML = header + buttons + listHtml;
    document.body.appendChild(container);

    document.getElementById('close-extractor').onclick = () => container.remove();
    document.getElementById('export-md').onclick = () => {
      const md = exportMarkdown(urls);
      download(md, `urls_${Date.now()}.md`, 'text/markdown');
    };
    document.getElementById('export-csv').onclick = () => {
      const csv = exportCsv(urls);
      download(csv, `urls_${Date.now()}.csv`, 'text/csv');
    };
  };

  try {
    const urls = extractUrls();
    if (urls.length === 0) {
        alert('No links found on this page.');
      return;
    }
    createUI(urls);
  } catch (error) {
      console.error('URL extractor error:', error);
      alert('Error extracting URLs.');
  }
})();