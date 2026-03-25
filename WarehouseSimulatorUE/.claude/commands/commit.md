변경된 파일을 확인하고 아래 절차대로 커밋해줘.

1. `git status` + `git diff` 로 변경 내용 파악
2. 관련 파일만 stage (`git add <파일>` — `git add .` 금지)
3. 커밋 메시지 규칙:
   - 형식: `<type>: <한국어 요약>`
   - type: feat / fix / refactor / docs / chore
   - 예) `feat: 팔레트 클릭 팝업 UI 구현`
4. 커밋 후 `git status` 로 확인

$ARGUMENTS 가 있으면 그 내용을 커밋 메시지 힌트로 사용해.
