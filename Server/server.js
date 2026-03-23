const express    = require('express');
const mysql      = require('mysql2/promise');
const cors       = require('cors');
const http       = require('http');
const WebSocket  = require('ws');

const app    = express();
const server = http.createServer(app);

// socket.io → ws (순수 WebSocket) 교체
// UE5 내장 WebSocket 모듈은 순수 ws 프로토콜만 지원
const wss = new WebSocket.Server({ server });

app.use(cors());
app.use(express.json());

// MySQL 연결 설정 - password 본인 비밀번호로 변경
const db = mysql.createPool({
    host:     'localhost',
    user:     'root',
    password: 'ahat0163@@',
    database: 'warehouse_db'
});

// 연결된 모든 클라이언트에게 이벤트 브로드캐스트
// 기존 io.emit(event, data) 역할
function broadcast(event, data) {
    const message = JSON.stringify({ event, data });
    wss.clients.forEach(client => {
        if (client.readyState === WebSocket.OPEN)
            client.send(message);
    });
}

// ───────────────────────────────────────────
// REST API
// ───────────────────────────────────────────

// 전체 컨테이너 조회
app.get('/containers', async (req, res) => {
    console.log('[HTTP] GET /containers');
    try
    {
        const [rows] = await db.query('SELECT * FROM containers');
        console.log(`[HTTP] 컨테이너 ${rows.length}개 반환`);
        res.json(rows);
    }
    catch (e)
    {
        res.status(500).json({ error: e.message });
    }
});

// 특정 슬롯 조회
app.get('/containers/:shelf/:floor/:slot', async (req, res) => {
    try
    {
        const { shelf, floor, slot } = req.params;
        const [rows] = await db.query(
            'SELECT * FROM containers WHERE shelf=? AND floor=? AND slot=?',
            [shelf, floor, slot]
        );
        res.json(rows[0] || null);
    }
    catch (e)
    {
        res.status(500).json({ error: e.message });
    }
});

// 컨테이너 입고
app.post('/containers', async (req, res) => {
    try
    {
        const { container_id, item_name, weight, arrival_date, shelf, floor, slot, width = 1.0, depth = 1.0, height = 1.0 } = req.body;
        await db.query(
            'INSERT INTO containers (container_id, item_name, weight, arrival_date, shelf, floor, slot, width, depth, height) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)',
            [container_id, item_name, weight, arrival_date, shelf, floor, slot, width, depth, height]
        );
        broadcast('containerAdded', req.body);
        res.json({ success: true });
    }
    catch (e)
    {
        res.status(500).json({ error: e.message });
    }
});

// 컨테이너 이동
app.patch('/containers/:id/move', async (req, res) => {
    try
    {
        const { id } = req.params;
        const { shelf, floor, slot } = req.body;
        await db.query(
            'UPDATE containers SET shelf=?, floor=?, slot=? WHERE container_id=?',
            [shelf, floor, slot, id]
        );
        broadcast('containerMoved', { container_id: id, shelf, floor, slot });
        res.json({ success: true });
    }
    catch (e)
    {
        res.status(500).json({ error: e.message });
    }
});

// 컨테이너 출고
app.delete('/containers/:id', async (req, res) => {
    try
    {
        const { id } = req.params;
        await db.query('DELETE FROM containers WHERE container_id=?', [id]);
        broadcast('containerRemoved', { container_id: id });
        res.json({ success: true });
    }
    catch (e)
    {
        res.status(500).json({ error: e.message });
    }
});

// ───────────────────────────────────────────
// WebSocket (실시간)
// ───────────────────────────────────────────
wss.on('connection', (ws) => {
    console.log('클라이언트 연결 (WebSocket)');
    ws.on('close', () => console.log('클라이언트 해제 (WebSocket)'));
});

// ───────────────────────────────────────────
// 서버 시작
// ───────────────────────────────────────────
const PORT = 3000;
server.listen(PORT, () => {
    console.log(`서버 실행 중: http://localhost:${PORT}`);
});
