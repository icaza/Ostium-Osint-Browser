(() => {
    'use strict';

    // ========== CONFIGURATION ==========
    const STOP_WORDS_EN = new Set([
        'a', 'an', 'and', 'are', 'as', 'at', 'be', 'by', 'for', 'from',
        'has', 'he', 'in', 'is', 'it', 'its', 'of', 'on', 'that', 'the',
        'to', 'was', 'were', 'will', 'with', 'i', 'you', 'your', 'we',
        'they', 'them', 'his', 'her', 'she', 'him', 'us', 'our', 'my',
        'me', 'or', 'but', 'not', 'no', 'so', 'if', 'then', 'than',
        'this', 'these', 'those', 'there', 'here', 'what', 'which', 'who',
        'whom', 'when', 'where', 'why', 'how', 'all', 'any', 'both',
        'each', 'few', 'more', 'most', 'other', 'some', 'such', 'only',
        'own', 'same', 'too', 'very', 'can', 'could', 'should', 'would',
        'may', 'might', 'must', 'shall', 'do', 'does', 'did', 'done',
        'doing', 'don', 'doesn', 'didn', 'isn', 'aren', 'wasn', 'weren',
        'hasn', 'haven', 'hadn', 'won', 'wouldn', 'shouldn', 'couldn',
        'can', 'cannot', 'can\'t', 'i\'m', 'you\'re', 'he\'s', 'she\'s',
        'it\'s', 'we\'re', 'they\'re', 'i\'ve', 'you\'ve', 'we\'ve',
        'they\'ve', 'i\'d', 'you\'d', 'he\'d', 'she\'d', 'we\'d', 'they\'d',
        'i\'ll', 'you\'ll', 'he\'ll', 'she\'ll', 'we\'ll', 'they\'ll'
    ]);

    const STOP_WORDS_FR = new Set([
        'à', 'afin', 'ai', 'aie', 'aient', 'aies', 'ainsi', 'ait', 'alors',
        'après', 'au', 'aucun', 'aucune', 'aussi', 'autre', 'autres', 'aux',
        'avaient', 'avais', 'avait', 'avec', 'avoir', 'avons', 'ayant', 'ayez', 'ayons',
        'beaucoup', 'bien', 'bon',
        'c', 'ça', 'ce', 'ceci', 'cela', 'celle', 'celles', 'celui', 'cependant',
        'certain', 'certaine', 'certaines', 'certains', 'ces', 'cet', 'cette', 'ceux',
        'chaque', 'chez', 'comme', 'comment', 'contre',
        'd', 'dans', 'de', 'dedans', 'dehors', 'depuis', 'des', 'deux', 'devrait',
        'doit', 'donc', 'dont', 'du', 'durant',
        'elle', 'elles', 'en', 'encore', 'entre', 'envers', 'es', 'est', 'et',
        'étaient', 'étais', 'était', 'étant', 'été', 'êtes', 'être',
        'eu', 'eue', 'eues', 'eux', 'eût', 'eut',
        'fais', 'faisaient', 'faisais', 'faisait', 'faisant', 'fait', 'faite', 'faites', 'faits',
        'faut', 'font', 'furent', 'fus', 'fût', 'fut',
        'gens', 'hors',
        'ici', 'il', 'ils',
        'j', 'je', 'jusqu',
        'l', 'la', 'là', 'le', 'les', 'leur', 'leurs', 'lorsque', 'lui',
        'm', 'ma', 'mais', 'malgré', 'me', 'même', 'mêmes', 'mes',
        'mien', 'mienne', 'miennes', 'miens', 'moi', 'moins', 'mon',
        'n', 'ne', 'ni', 'nos', 'notre', 'nous', 'nôtre', 'nôtres',
        'on', 'ont', 'ou', 'où',
        'par', 'parce', 'parmi', 'pas', 'pendant', 'personne', 'peu', 'plus', 'plusieurs',
        'pour', 'pourquoi', 'près', 'presque', 'puis',
        'qu', 'quand', 'que', 'quel', 'quelle', 'quelles', 'quelqu', 'quelque', 'quelques',
        'quels', 'qui', 'quoi', 'quoique',
        'rien',
        's', 'sa', 'sans', 'se', 'selon', 'sera', 'seraient', 'serais', 'serait',
        'seras', 'serez', 'seriez', 'serions', 'seront', 'ses', 'si', 'soi', 'soient',
        'sois', 'soit', 'sommes', 'son', 'sont', 'sous', 'suis', 'sur',
        't', 'ta', 'tandis', 'tant', 'te', 'tel', 'telle', 'telles', 'tels', 'tes',
        'toi', 'ton', 'tous', 'tout', 'toute', 'toutes', 'très', 'trop', 'tu',
        'un', 'une', 'uns', 'unes',
        'va', 'vais', 'vas', 'vers', 'veut', 'veux', 'voient', 'vois', 'voit',
        'vont', 'vos', 'votre', 'vôtre', 'vôtres', 'vous',
        'y',
        'c\'est', 's\'est', 'n\'est', 'n\'était', 'n\'étais', 'n\'étiez', 'n\'étions', 'n\'étaient',
        'l\'on', 'j\'ai', 'j\'avais', 'j\'aurais', 'qu\'il', 'qu\'elle', 'qu\'on', 'qu\'un',
        'd\'un', 'd\'une', 'jusqu\'à'
    ]);

    const STOP_WORDS = new Set([...STOP_WORDS_EN, ...STOP_WORDS_FR]);

    const STOP_WORDS_BY_LANG = {
        en: STOP_WORDS_EN,
        fr: STOP_WORDS_FR,
    };

    // ========== STATE ==========
    const settings = {
        removeStopWords: true,
        minLength: 3,
        maxWords: 100
    };

    // ========== DOM REFS ==========
    let host, shadowRoot;
    let wordCloudContainer, statsContainer;
    let chkStopWords, numMinLength, numMaxWords, btnAnalyze;
    let lastText = '';

    // ========== TEXT EXTRACTION ==========
    function getPageText() {
        return document.body.innerText || document.body.textContent || '';
    }

    // ========== ANALYSIS ==========
    function analyze(text) {
        const tokens = text.toLowerCase().match(/\p{L}(?:\p{L}|'|-)*/gu) || [];
        const totalTokens = tokens.length;

        const filteredTokens = tokens.filter(word =>
            word.length >= settings.minLength &&
            (!settings.removeStopWords || !STOP_WORDS.has(word.toLowerCase()))
        );

        const freqMap = new Map();
        for (const word of filteredTokens) {
            freqMap.set(word, (freqMap.get(word) || 0) + 1);
        }

        const totalFiltered = filteredTokens.length;
        const uniqueWords = freqMap.size;
        const sorted = [...freqMap.entries()]
            .sort((a, b) => b[1] - a[1])
            .slice(0, settings.maxWords);

        return { totalTokens, totalFiltered, uniqueWords, sorted };
    }

    // ========== RENDER ==========
    function renderWordCloud(sorted, totalFiltered) {
        wordCloudContainer.textContent = '';

        if (!sorted.length) {
            const emptyMsg = document.createElement('div');
            emptyMsg.className = 'empty-msg';
            emptyMsg.textContent = 'No words match the current filters.';
            wordCloudContainer.appendChild(emptyMsg);
            return;
        }

        const maxFreq = sorted[0][1];
        const minFreq = sorted[sorted.length - 1][1];

        sorted.forEach(([word, freq]) => {
            const span = document.createElement('span');
            span.className = 'word';
            span.textContent = word;
            span.title = `Frequency: ${freq} (${((freq / totalFiltered) * 100).toFixed(2)}%)`;

            const fontSize = 12 + ((freq - minFreq) / (maxFreq - minFreq || 1)) * 60;
            span.style.fontSize = `${fontSize}px`;

            const ratio = (freq - minFreq) / (maxFreq - minFreq || 1);
            const hue = 180 + ratio * 120;
            span.style.color = `hsl(${hue}, 100%, 70%)`;

            wordCloudContainer.appendChild(span);
        });
    }

    function renderStats(analysis) {
        statsContainer.textContent = '';

        const stats = [
            { label: 'Total tokens', value: analysis.totalTokens },
            { label: 'Filtered words', value: analysis.totalFiltered },
            { label: 'Unique words', value: analysis.uniqueWords },
            { label: 'Top word', value: analysis.sorted[0]?.[0] || 'N/A' }
        ];

        stats.forEach(stat => {
            const row = document.createElement('div');
            row.className = 'stat-row';
            const label = document.createElement('span');
            label.className = 'stat-label';
            label.textContent = stat.label;
            const value = document.createElement('span');
            value.className = 'stat-value';
            value.textContent = stat.value;
            row.appendChild(label);
            row.appendChild(value);
            statsContainer.appendChild(row);
        });
    }

    function runAnalysis() {
        const text = getPageText();
        lastText = text;
        const analysis = analyze(text);
        renderStats(analysis);
        renderWordCloud(analysis.sorted, analysis.totalFiltered);
    }

    // ========== UI CONSTRUCTION ==========
    function createUI() {
        host = document.createElement('div');
        host.id = 'word-cloud-extension-root';
        host.style.cssText = `
      position: fixed;
      top: 20px;
      right: 20px;
      width: 380px;
      max-height: 80vh;
      z-index: 2147483647;
      font-family: 'Segoe UI', 'Roboto', 'Helvetica Neue', sans-serif;
      letter-spacing: 0.5px;
    `;

        shadowRoot = host.attachShadow({ mode: 'open' });

        const style = document.createElement('style');
        style.textContent = `
      :host {
        all: initial;
      }
      .container {
        background: rgba(13, 13, 20, 0.95);
        border: 1px solid rgba(0, 255, 255, 0.3);
        border-radius: 12px;
        box-shadow: 0 0 20px rgba(0, 255, 255, 0.2), inset 0 0 10px rgba(0, 255, 255, 0.05);
        backdrop-filter: blur(10px);
        overflow: hidden;
        color: #e0e0e0;
        display: flex;
        flex-direction: column;
        max-height: 80vh;
      }
      .header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        padding: 12px 16px;
        background: rgba(0, 255, 255, 0.05);
        border-bottom: 1px solid rgba(0, 255, 255, 0.2);
        cursor: move;
      }
      .title {
        font-size: 1.2rem;
        font-weight: bold;
        text-transform: uppercase;
        letter-spacing: 2px;
        color: #00ffff;
        text-shadow: 0 0 8px rgba(0, 255, 255, 0.7);
      }
      .close-btn {
        background: none;
        border: none;
        color: #ff4d6d;
        font-size: 1.5rem;
        cursor: pointer;
        line-height: 1;
        padding: 0 4px;
        transition: color 0.2s;
      }
      .close-btn:hover {
        color: #ff1a4d;
        text-shadow: 0 0 8px #ff4d6d;
      }
      .controls {
        padding: 12px 16px;
        display: flex;
        flex-wrap: wrap;
        gap: 10px;
        align-items: center;
        border-bottom: 1px solid rgba(0, 255, 255, 0.2);
      }
      .control-group {
        display: flex;
        align-items: center;
        gap: 6px;
        font-size: 0.85rem;
      }
      input[type="number"] {
        width: 60px;
        background: rgba(0, 0, 0, 0.4);
        border: 1px solid rgba(0, 255, 255, 0.3);
        border-radius: 4px;
        color: #fff;
        padding: 4px 6px;
        font-size: 0.85rem;
      }
      input[type="checkbox"] {
        accent-color: #00ffff;
        cursor: pointer;
      }
      .btn {
        background: rgba(0, 255, 255, 0.1);
        border: 1px solid #00ffff;
        border-radius: 6px;
        color: #00ffff;
        padding: 6px 12px;
        cursor: pointer;
        font-size: 0.85rem;
        text-transform: uppercase;
        letter-spacing: 1px;
        transition: all 0.2s;
      }
      .btn:hover {
        background: rgba(0, 255, 255, 0.25);
        box-shadow: 0 0 12px rgba(0, 255, 255, 0.5);
      }
      .stats {
        padding: 10px 16px;
        display: grid;
        grid-template-columns: 1fr 1fr;
        gap: 8px;
        border-bottom: 1px solid rgba(0, 255, 255, 0.2);
        background: rgba(0, 0, 0, 0.2);
      }
      .stat-row {
        display: flex;
        justify-content: space-between;
        font-size: 0.85rem;
      }
      .stat-label {
        color: #888;
      }
      .stat-value {
        color: #00ffff;
        font-weight: bold;
        text-shadow: 0 0 6px rgba(0, 255, 255, 0.5);
      }
      .word-cloud {
        flex: 1;
        overflow-y: auto;
        padding: 16px;
        display: flex;
        flex-wrap: wrap;
        gap: 10px 16px;
        align-items: center;
        justify-content: center;
        align-content: flex-start;
      }
      .word {
        cursor: default;
        transition: transform 0.2s, text-shadow 0.2s;
        line-height: 1.2;
        white-space: nowrap;
      }
      .word:hover {
        transform: scale(1.1);
        text-shadow: 0 0 10px currentColor;
      }
      .empty-msg {
        color: #666;
        font-style: italic;
      }
      .word-cloud::-webkit-scrollbar {
        width: 6px;
      }
      .word-cloud::-webkit-scrollbar-track {
        background: rgba(0, 0, 0, 0.2);
      }
      .word-cloud::-webkit-scrollbar-thumb {
        background: rgba(0, 255, 255, 0.5);
        border-radius: 3px;
      }
    `;

        const container = document.createElement('div');
        container.className = 'container';

        const header = document.createElement('div');
        header.className = 'header';
        const title = document.createElement('span');
        title.className = 'title';
        title.textContent = 'Word Cloud Analyzer';
        const closeBtn = document.createElement('button');
        closeBtn.className = 'close-btn';
        closeBtn.innerHTML = '&times;';
        closeBtn.title = 'Close';
        closeBtn.addEventListener('click', () => host.remove());
        header.appendChild(title);
        header.appendChild(closeBtn);

        const controls = document.createElement('div');
        controls.className = 'controls';

        const chkGroup = document.createElement('div');
        chkGroup.className = 'control-group';
        chkStopWords = document.createElement('input');
        chkStopWords.type = 'checkbox';
        chkStopWords.id = 'chk-stopwords';
        chkStopWords.checked = settings.removeStopWords;
        chkStopWords.addEventListener('change', (e) => {
            settings.removeStopWords = e.target.checked;
        });
        const chkLabel = document.createElement('label');
        chkLabel.htmlFor = 'chk-stopwords';
        chkLabel.textContent = 'Remove stop words';
        chkGroup.appendChild(chkStopWords);
        chkGroup.appendChild(chkLabel);

        const lenGroup = document.createElement('div');
        lenGroup.className = 'control-group';
        const lenLabel = document.createElement('label');
        lenLabel.htmlFor = 'num-minlength';
        lenLabel.textContent = 'Min length:';
        numMinLength = document.createElement('input');
        numMinLength.type = 'number';
        numMinLength.id = 'num-minlength';
        numMinLength.min = 1;
        numMinLength.value = settings.minLength;
        numMinLength.addEventListener('change', () => {
            const val = parseInt(numMinLength.value, 10);
            if (!isNaN(val) && val >= 1) settings.minLength = val;
            else numMinLength.value = settings.minLength;
        });
        lenGroup.appendChild(lenLabel);
        lenGroup.appendChild(numMinLength);

        const maxGroup = document.createElement('div');
        maxGroup.className = 'control-group';
        const maxLabel = document.createElement('label');
        maxLabel.htmlFor = 'num-maxwords';
        maxLabel.textContent = 'Max words:';
        numMaxWords = document.createElement('input');
        numMaxWords.type = 'number';
        numMaxWords.id = 'num-maxwords';
        numMaxWords.min = 1;
        numMaxWords.max = 1000;
        numMaxWords.value = settings.maxWords;
        numMaxWords.addEventListener('change', () => {
            const val = parseInt(numMaxWords.value, 10);
            if (!isNaN(val) && val >= 1) settings.maxWords = Math.min(val, 1000);
            else numMaxWords.value = settings.maxWords;
        });
        maxGroup.appendChild(maxLabel);
        maxGroup.appendChild(numMaxWords);

        btnAnalyze = document.createElement('button');
        btnAnalyze.className = 'btn';
        btnAnalyze.textContent = 'Analyze';
        btnAnalyze.addEventListener('click', runAnalysis);

        controls.appendChild(chkGroup);
        controls.appendChild(lenGroup);
        controls.appendChild(maxGroup);
        controls.appendChild(btnAnalyze);

        statsContainer = document.createElement('div');
        statsContainer.className = 'stats';

        wordCloudContainer = document.createElement('div');
        wordCloudContainer.className = 'word-cloud';

        container.appendChild(header);
        container.appendChild(controls);
        container.appendChild(statsContainer);
        container.appendChild(wordCloudContainer);

        shadowRoot.appendChild(style);
        shadowRoot.appendChild(container);

        document.body.appendChild(host);

        makeDraggable(host, header);
    }

    // ========== DRAGGABLE ==========
    function makeDraggable(element, handle) {
        let pos1 = 0, pos2 = 0, pos3 = 0, pos4 = 0;
        handle.onpointerdown = dragMouseDown;

        function dragMouseDown(e) {
            e.preventDefault();
            pos3 = e.clientX;
            pos4 = e.clientY;
            document.onpointerup = closeDragElement;
            document.onpointermove = elementDrag;
        }

        function elementDrag(e) {
            e.preventDefault();
            pos1 = pos3 - e.clientX;
            pos2 = pos4 - e.clientY;
            pos3 = e.clientX;
            pos4 = e.clientY;
            element.style.top = (element.offsetTop - pos2) + 'px';
            element.style.left = (element.offsetLeft - pos1) + 'px';
            element.style.right = 'auto';
        }

        function closeDragElement() {
            document.onpointerup = null;
            document.onpointermove = null;
        }
    }

    // ========== INIT ==========
    function init() {
        if (document.getElementById('word-cloud-extension-root')) {
            console.warn('Word Cloud Analyzer already running.');
            return;
        }
        createUI();
        runAnalysis();
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }
})();