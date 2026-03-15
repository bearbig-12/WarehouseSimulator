const express   = require('express');
const mysql     = require('mysql2/promise');
const cors      = require('cors');
const http      = require('http');
const { Server } = require('socket.io');

const app    = express();
const server = http.createServer(app);
const io     = new Server(server, { cors: { origin: '*' } });

app.use(cors());
app.use(express.json());

// MySQL 연결 설정 - password 본인 비밀번호로 변경
const db = mysql.createPool({
    host:     'localhost',
    user:     'root',
    password: 'ahat0163@@',
    database: 'warehouse_db'
});

// ───────────────────────────────────────────
// REST API
// ───────────────────────────────────────────

// 전체 컨테이너 조회
app.get('/containers', async (req, res) => {
    try {
        const [rows] = await db.query('SELECT * FROM containers');
        res.json(rows);
    } catch (e) { res.status(500).json({ error: e.message }); }
});

// 특정 슬롯 조회
app.get('/containers/:shelf/:floor/:slot', async (req, res) => {
    try {
        const { shelf, floor, slot } = req.params;
        const [rows] = await db.query(
            'SELECT * FROM containers WHERE shelf=? AND floor=? AND slot=?',
            [shelf, floor, slot]
        );
        res.json(rows[0] || null);
    } catch (e) { res.status(500).json({ error: e.message }); }
});

// 컨테이너 입고
app.post('/containers', async (req, res) => {
    try {
        const { container_id, item_name, weight, arrival_date, shelf, floor, slot } = req.body;
        await db.query(
            'INSERT INTO containers (container_id, item_name, weight, arrival_date, shelf, floor, slot) VALUES (?, ?, ?, ?, ?, ?, ?)',
            [container_id, item_name, weight, arrival_date, shelf, floor, slot]
        );
        io.emit('containerAdded', req.body);
        res.json({ success: true });
    } catch (e) { res.status(500).json({ error: e.message }); }
});

// 컨테이너 이동
app.patch('/containers/:id/move', async (req, res) => {
    try {
        const { id } = req.params;
        const { shelf, floor, slot } = req.body;
        await db.query(
            'UPDATE containers SET shelf=?, floor=?, slot=? WHERE container_id=?',
            [shelf, floor, slot, id]
        );
        io.emit('containerMoved', { container_id: id, shelf, floor, slot });
        res.json({ success: true });
    } catch (e) { res.status(500).json({ error: e.message }); }
});

// 컨테이너 출고
app.delete('/containers/:id', async (req, res) => {
    try {
        const { id } = req.params;
        await db.query('DELETE FROM containers WHERE container_id=?', [id]);
        io.emit('containerRemoved', { container_id: id });
        res.json({ success: true });
    } catch (e) { res.status(500).json({ error: e.message }); }
});

// ───────────────────────────────────────────
// Socket.io (실시간)
// ───────────────────────────────────────────
io.on('connection', (socket) => {
    console.log('클라이언트 연결:', socket.id);
    socket.on('disconnect', () => console.log('클라이언트 해제:', socket.id));
});

// ───────────────────────────────────────────
// 서버 시작
// ───────────────────────────────────────────
const PORT = 3000;
server.listen(PORT, () => {
    console.log(`서버 실행 중: http://localhost:${PORT}`);
});
