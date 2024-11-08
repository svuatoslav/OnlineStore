﻿using Npgsql;
using OnlineStore;
using OnlineStore.Models;
using OnlineStore.XMLeditor;
using OnlineStore.BDeditor;
using System.Xml;
using System.Xml.Linq;

Console.WriteLine("Введите строку для подключения к базе данных в формате:\n Host=localhost;Port=NumberPort;Database=DatabaseName;Username=postgres;Password=password");
string pathXML = "XMLFiles\\DATA.xml";
string? connect = Console.ReadLine();

IXmlDocument xmlDocument = new XMLXDocument();
IXmlReader xmlReader = new XMLReader();
IWriteBD writeBD = new BDSave();
IClear clearing = new DataClear();

DataTransmission dataTransmission = new(xmlDocument, writeBD, clearing);
if (connect != null )
    dataTransmission.ParseXmlToStoreData(dataTransmission, connect, pathXML);
Console.WriteLine();