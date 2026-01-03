# 📊 Corporate Dashboard Automator

![Badge Status](https://img.shields.io/badge/Status-Active-brightgreen)
![Badge .NET](https://img.shields.io/badge/.NET-Core%2F8-purple)
![Badge Selenium](https://img.shields.io/badge/Selenium-WebDriver-43B02A)
![Badge License](https://img.shields.io/badge/License-MIT-blue)

> **Mantenha seus KPIs sempre visíveis. Elimine o "F5" manual.**

Este projeto é uma solução robusta desenvolvida em **C#** para automatizar a rotação e exibição de painéis de Business Intelligence (Power BI, Tableau, Qlik, etc.) em monitores corporativos e Videowalls.

---

## 🚀 O Problema
Em ambientes corporativos, manter dashboards visíveis em TVs requer login manual constante, atualização de páginas e gestão de sessões que expiram. Isso gera telas pretas, dados desatualizados e perda de tempo da equipe de TI/Gestão.

## 💡 A Solução
Um ecossistema completo que gerencia, orquestra e executa a exibição dos dados sem intervenção humana, garantindo que a gestão tenha acesso contínuo às métricas vitais da empresa.

---

## 🏗️ Arquitetura do Projeto

O sistema é dividido em três pilares para garantir escalabilidade e fácil gerenciamento:

### 1. 🖥️ Web Management (ASP.NET MVC)
O "Painel de Controle". Uma interface amigável onde o administrador pode:
* Cadastrar e gerenciar as URLs dos Dashboards.
* Definir o tempo de exibição (rotação) de cada painel.
* Configurar credenciais de acesso de forma segura.

### 2. 🔗 Core API (ASP.NET Web API)
O "Cérebro". Atua como a ponte entre o gerenciador web e os robôs de exibição.
* Fornece os dados de configuração para os agentes (Console).
* Recebe logs de status e saúde dos monitores.

### 3. 🤖 Display Agent (Console App + Selenium)
O "Executor". Uma aplicação leve que roda nas máquinas conectadas às TVs.
* Utiliza **Selenium WebDriver** para abrir o browser em modo fullscreen (tela cheia).
* Realiza o login automático nas plataformas de BI.
* Gerencia a rotação das abas e o refresh dos dados conforme configurado na API.

---

## 🛠️ Tecnologias Utilizadas

* **Linguagem:** C# (.NET)
* **Automação Web:** Selenium WebDriver
* **Backend/Frontend:** ASP.NET Core (MVC & Web API)
* **Banco de Dados:** Oracle PLSQL
* **Estilização:** Bootstrap / CSS3 / AJAX / JQUERY

  ---

## 🛠️ Metodoloias Utilizadas

* **Arquitetura:** Camadas
* **Padrão:** Repository Pattern
* **API:** Minimal APIs
