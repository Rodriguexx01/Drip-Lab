# Drip Lab

Projeto de TCC: jogo de gestão de uma marca de streetwear, desenvolvido na Unity para Windows e WebGL.

## Abrir o projeto

1. Instale a Unity Editor **6000.6.6f1** pelo Unity Hub.
2. No Unity Hub, adicione esta pasta como projeto.
3. Abra `Assets/Scenes/Atelie.unity` e clique em **Play**.

## Organização

- `Assets/Scripts`: regras, estado do jogo, telas, interação e áudio.
- `Assets/Scenes`: cena principal.
- `Assets/Resources/UI`: telas, botões e interface.
- `Assets/Resources/Bench`: imagens do ateliê e da bancada.
- `Assets/Resources/Story`: imagens da história.
- `Assets/Resources/Audio`: música e efeitos sonoros.
- `Assets/Editor`: ferramentas de edição e exportação.
- `Packages` e `ProjectSettings`: dependências e configurações da Unity.

As pastas `Library`, `Temp`, `Logs` e `UserSettings` são geradas localmente pela Unity e não fazem parte do código-fonte. As imagens e os áudios são armazenados com Git LFS; ao clonar, mantenha o Git LFS instalado para receber os arquivos completos.

## Exportar

Na Unity, use o menu **DripLab > Gerar jogo Web** ou **DripLab > Gerar Windows otimizado**. As exportações são criadas ao lado da pasta do projeto. A build Windows precisa manter o executável e a pasta `Drip Lab_Data` juntos.
