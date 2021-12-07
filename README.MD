# 설명

**생성 방법**
1. MyFrameWork GameObject를 선택해서 인스펙터창에 있는 생성 버튼으로 생성
2. 게임 시작시 자동으로 생성

![생성버튼1](https://github.com/SooWanKim/MakeBuildingInUnity/blob/master/%EB%85%B9%ED%99%94_2021_06_29_19_10_45_813.gif)

![생성버튼2](https://github.com/SooWanKim/MakeBuildingInUnity/blob/master/%EB%85%B9%ED%99%94_2021_06_29_19_11_33_888.gif)


# 문제 해결 방법

생성된 메시의 정점의 normal값과 Vector3.forward 방향으로 180<= x && x <=220을 충족하는 방향이 잘 안나와서
생성할때 직접 front, back, left, right면으로 uv 세팅하는것과 각도로 세팅하는것 2개로 만들었습니다. 각도로 세팅하는건 제대로 맵핑이 되지 않습니다.

주어진 폴리곤 정보로 메시를 생성할때 건물이 box형태라고 가정하여, 바닥4개 정점, 탑4개 정점을 추출하여 박스를 생성하였습니다.

다수의 건물생성에 부하가 발생할 수 있어 async/ await 로 생성하였습니다.

