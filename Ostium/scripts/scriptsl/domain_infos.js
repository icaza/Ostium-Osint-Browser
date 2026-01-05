/**
 * @file Domain_Infos.js
 * @author ICAZA <github.wildly512@passinbox.com>
 * @version 1.1
 * @description Display information for a domain.
 * @license MIT
 */

function collectData() {
    return {
        "Full domain name": window.location.hostname,
        "Protocol": window.location.protocol,
        "Subdomain": window.location.hostname.split('.')[0],
        "TLD": window.location.hostname.split('.').pop(),
        "Port": window.location.port || (window.location.protocol === 'https:' ? '443' : '80'),
        "Path Name": window.location.pathname,
        "URL parameters": window.location.search,
        "Anchor URL": window.location.hash,
        "Domain Origin": window.location.origin,
        "Cookies": document.cookie
    };
}

function createTable(data) {
    let table = `
        <table style="width: 100%; border-collapse: collapse; margin-top: 20px;">
            <tr>
                <th style="border: 1px solid #ddd; padding: 8px; background-color: #f2f2f2; font-weight: bold; text-align: left;">Property</th>
                <th style="border: 1px solid #ddd; padding: 8px; background-color: #f2f2f2; font-weight: bold; text-align: left;">Value</th>
            </tr>
    `;
    for (let [key, value] of Object.entries(data)) {
        table += `
            <tr>
                <td style="border: 1px solid #ddd; padding: 8px;">${key}</td>
                <td style="border: 1px solid #ddd; padding: 8px; word-break: break-all;">${value || 'N/A'}</td>
            </tr>
        `;
    }
    table += '</table>';
    return table;
}

document.body.insertAdjacentHTML('afterbegin', `
    <div id="dataContainer" style="margin: 20px; font-family: Arial, sans-serif;">
        <h2 style="color: #333;">Domain information</h2>
        ${createTable(collectData())}
    </div>
`);
