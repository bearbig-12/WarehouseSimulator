feature 브랜치를 만들고 작업 후 PR 머지까지 진행해줘.

$ARGUMENTS 를 브랜치 이름/작업 설명으로 사용해.

절차:
1. `git checkout -b feature/$ARGUMENTS` 로 브랜치 생성
2. 요청된 코드 변경 작업
3. 변경 파일만 stage 후 한국어 커밋
4. `git push -u origin feature/$ARGUMENTS`
5. `gh pr create` 로 PR 생성 (한국어 제목/설명)
6. `gh pr merge --merge` 로 머지
7. `git checkout main && git pull` 로 복귀

브랜치 이름이 없으면 작업 내용에서 자동으로 이름 추론해줘.
