# hwp2pdf

한컴오피스 한/글 자동화 기능을 사용하여 문서를 일괄 변환하는 Windows Forms 프로그램입니다.

## 개발 환경

- C# / Windows Forms
- Microsoft Visual Studio
- .NET Framework 4.8
- 대상 플랫폼: 프로젝트 설정에 따름 (`x86` 또는 `x64`)

## 실행 요구사항

- Windows
- 한컴오피스 한/글 2010 이상
- 빌드 시 한컴오피스 자동화 COM 구성 요소
- PDF 변환 시 다음 중 하나의 프린터
  - 한컴 PDF
  - Microsoft Print to PDF

한/글 자동화에서 로컬 파일에 접근하려면 `FilePathCheckerModuleExample.DLL`이 필요할 수
있습니다. DLL을 실행 파일과 같은 폴더에 배치하면 프로그램이 레지스트리에 모듈 경로를
등록합니다.

## 빌드

1. `hwp2pdf.sln`을 Visual Studio에서 엽니다.
2. 한컴오피스가 설치된 환경에서 `hwp2pdf` 프로젝트를 빌드합니다.
3. 빌드 결과 폴더에 필요한 한컴오피스 Interop DLL과
   `FilePathCheckerModuleExample.DLL`을 배치합니다.

프로젝트는 .NET Framework 4.8을 대상으로 합니다. 한컴오피스 COM 참조는 개발 및 실행
환경의 한컴오피스 설치 상태에 따라 달라질 수 있습니다.

## 사용 방법

1. `hwp2pdf.exe`를 실행합니다.
2. 문서를 목록으로 드래그 앤 드롭하거나 파일 추가 메뉴를 사용합니다.
3. 저장 경로와 대상 형식을 선택합니다.
4. `변환 시작`을 클릭합니다.

지원 입력 형식:

- `.hwp`, `.hwpx`, `.hml`
- `.html`, `.odt`
- `.docx`, `.doc`, `.txt`, `.rtf`

지원 출력 형식:

- PDF (`.pdf`)
- HWP (`.hwp`)
- HWPX (`.hwpx`)
- HWPML2X (`.hml`)
- HTML+ (`.html`)
- ODT (`.odt`)
- OOXML (`.docx`)
- MSWORD (`.doc`)
- UNICODE (`.txt`)
- RTF (`.rtf`)

## 저장 및 중복 파일 처리

- 기본 저장 위치는 원본 파일과 같은 폴더입니다.
- 설정에서 별도의 저장 폴더를 지정할 수 있습니다.
- 같은 이름의 파일이 있으면 다음 정책 중 하나를 선택할 수 있습니다.
  - 새 이름으로 저장
  - 변환하지 않고 건너뛰기
  - 기존 파일 덮어쓰기
- PDF는 `SaveAs` 방식 또는 설치된 PDF 가상 프린터 방식으로 변환할 수 있습니다.

## 문제 해결

### 파일 접근 권한 오류

한/글 자동화 중 파일 접근 권한 경고가 표시되면 경고 창에서 `모두 허용`을 선택합니다.
`FilePathCheckerModuleExample.DLL`이 실행 파일과 같은 폴더에 있는지 확인하고, 레지스트리
등록 권한이 없는 경우 관리자 권한으로 실행합니다.

### PDF 파일이 생성되지 않는 경우

- 한컴 PDF 또는 Microsoft Print to PDF가 설치되어 있는지 확인합니다.
- 설정에서 선택한 프린터가 실제 설치된 프린터인지 확인합니다.
- 출력 폴더에 쓰기 권한이 있는지 확인합니다.
- 변환 상태에 출력 파일 확인 시간 초과가 표시되면 프린터 및 출력 경로를 점검합니다.
