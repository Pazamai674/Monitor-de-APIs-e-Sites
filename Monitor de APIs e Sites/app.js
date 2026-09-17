const ctx = document.getElementById('latenciaChart').getContext('2d');
const chart = new Chart(ctx, {
    type: 'line',
    data: {
        labels: [],
        datasets: []
    },
    options: {
        responsive: true,
        scales: {
            y: { beginAtZero: true, title: { display: true, text: 'Latência (ms)' } }
        }
    }
});


const connection = new signalR.HubConnectionBuilder()
    .withUrl("/monitorHub")
    .withAutomaticReconnect()
    .build();

connection.on("AtualizacaoSite", (resultado) => {
    atualizarCard(resultado);
    atualizarGrafico(resultado);
});

connection.start()
    .then(() => console.log("Conectado ao SignalR!"))
    .catch(err => console.error("Erro no SignalR: ", err));


function atualizarCard(res) {
    let card = document.getElementById(`site-card-${res.siteId}`);
    const sitesGrid = document.getElementById('sitesGrid');

    const statusColor = res.online ? 'bg-green-600' : 'bg-red-600';
    const statusText = res.online ? 'ONLINE' : 'OFFLINE';

    if (!card) {
        card = document.createElement('div');
        card.id = `site-card-${res.siteId}`;
        card.className = "p-4 rounded-lg bg-gray-800 border border-gray-700 shadow";
        sitesGrid.appendChild(card);
    }

    card.innerHTML = `
        <div class="flex justify-between items-center mb-2">
            <h3 class="font-bold text-lg">${res.nome}</h3>
            <span class="${statusColor} text-xs font-bold px-2 py-1 rounded">${statusText}</span>
        </div>
        <p class="text-sm text-gray-400">Latência: <span class="text-white font-mono">${res.latenciaMs} ms</span></p>
        <p class="text-xs text-gray-500 mt-2">Checado às: ${new Date(res.timestamp).toLocaleTimeString()}</p>
    `;
}

function atualizarGrafico(res) {
    const horaFormatada = new Date(res.timestamp).toLocaleTimeString();

    if (!chart.data.labels.includes(horaFormatada)) {
        chart.data.labels.push(horaFormatada);
        if (chart.data.labels.length > 10) chart.data.labels.shift();
    }

    let dataset = chart.data.datasets.find(ds => ds.label === res.nome);
    if (!dataset) {
        const cores = ['#3b82f6', '#10b981', '#ef4444', '#f59e0b'];
        const cor = cores[chart.data.datasets.length % cores.length];
        
        dataset = {
            label: res.nome,
            data: [],
            borderColor: cor,
            backgroundColor: cor,
            tension: 0.2
        };
        chart.data.datasets.push(dataset);
    }

    dataset.data.push(res.latenciaMs);
    if (dataset.data.length > 10) dataset.data.shift();

    chart.update();
}