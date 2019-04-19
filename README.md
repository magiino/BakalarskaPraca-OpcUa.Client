# OPC UA Client
Tento projekt bol vytvorený v rámci bakalárskej práce: https://www.vutbr.cz/studenti/zav-prace?zp_id=110733

V tomto projekte som vytvoril desktop aplikáciu **OPC UA klient-a**. Aplikácia je postavená na **.NET platforme 4.6.2** s využitím **C#** a grafické rozhranie pomocou technológie **WPF**. Aplikácia je postavená na **MVVM** návrhovom vzore. Vo VIEW-MODELoch používam **PropertyChanged.Fody** package, ktorý za mňa notifikuje view o zmene property vo view modele. Na komunikáciu medzi VIEW-MODELmi používam návrhový vzor **messenger**. Pre ukladanie dát používam **MS SQL** databázu, ku ktorej pristupojem pomocou ORM **Entiti Framework 6.2**. Pre lepšiu vyzuálnu stránku aplikácie využívam **MaterialDesign** WPF prvky. Taktiež využívam **Ninject** ako **IoC** kontajner. Pre vykreslovanie grafov som použil balíček **LiveCharts**. Pre protokol **OPC UA** som použil oficiálnu .NET implementáciu od **OPC Foundation** - https://github.com/OPCFoundation/UA-.NET-Legacy .

#### Funckie aplikácie:
* Možnosť filtrovať nájdené servery cez LDS server
* Po vybratí servera zo zoznamu, možnosť filtrovať nájdené endpointy pre daný OPC UA server
* Podporovaná OPC UA autentifikácia - *Anonymné* a *Meno/heslo*
* Grafické prehladávanie OPC UA adresového priestoru
* Info k danému uzlu označeného v adresovom priestore
* Nastavenie notifikácii na zmenu digitálnej hodnoty alebo analógovej pre ktorú je možné nastaviť *treshold*
* Archív
* Vykreslenie archivovaných hodnôt v grafoch
* Sledovanie aktuálnych zmien daných premenných v grafe

Aplikáciu som testoval s rôznymi *free* OPC UA servermi od rôznych spoločností a na OPC UA servery zabudovanom v PLC od spoločnosti Siemens: S7-1500.

[PLC program](https://github.com/magiino/BakalarskaPraca-Triedenie_Bedni). 

Podrobnejšie informáciie nájdete v bakalárskej práci - https://www.vutbr.cz/studenti/zav-prace?zp_id=110733

V prípade otázok ma môžete kontaktovať na - marekk.magath@gmail.com
