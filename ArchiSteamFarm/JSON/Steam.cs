﻿/*
    _                _      _  ____   _                           _____
   / \    _ __  ___ | |__  (_)/ ___| | |_  ___   __ _  _ __ ___  |  ___|__ _  _ __  _ __ ___
  / _ \  | '__|/ __|| '_ \ | |\___ \ | __|/ _ \ / _` || '_ ` _ \ | |_  / _` || '__|| '_ ` _ \
 / ___ \ | |  | (__ | | | || | ___) || |_|  __/| (_| || | | | | ||  _|| (_| || |   | | | | | |
/_/   \_\|_|   \___||_| |_||_||____/  \__|\___| \__,_||_| |_| |_||_|   \__,_||_|   |_| |_| |_|

 Copyright 2015-2016 Łukasz "JustArchi" Domeradzki
 Contact: JustArchi@JustArchi.net

 Licensed under the Apache License, Version 2.0 (the "License");
 you may not use this file except in compliance with the License.
 You may obtain a copy of the License at

 http://www.apache.org/licenses/LICENSE-2.0
					
 Unless required by applicable law or agreed to in writing, software
 distributed under the License is distributed on an "AS IS" BASIS,
 WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 See the License for the specific language governing permissions and
 limitations under the License.

*/

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using HtmlAgilityPack;
using Newtonsoft.Json;
using SteamKit2;

namespace ArchiSteamFarm.JSON {
	internal static class Steam {
		internal sealed class Item { // REF: https://developer.valvesoftware.com/wiki/Steam_Web_API/IEconService#CEcon_Asset
			internal const ushort SteamAppID = 753;
			internal const byte SteamContextID = 6;

			internal enum EType : byte {
				Unknown,

				BoosterPack,
				Coupon,
				Gift,
				SteamGems,

				Emoticon,
				FoilTradingCard,
				ProfileBackground,
				TradingCard
			}

			internal uint AppID { get; set; }

			[JsonProperty(PropertyName = "appid", Required = Required.DisallowNull)]
			[SuppressMessage("ReSharper", "UnusedMember.Local")]
			private string AppIDString {
				get {
					return AppID.ToString();
				}

				set {
					if (string.IsNullOrEmpty(value)) {
						return;
					}

					uint result;
					if (!uint.TryParse(value, out result)) {
						return;
					}

					AppID = result;
				}
			}

			internal ulong ContextID { get; set; }

			[JsonProperty(PropertyName = "contextid", Required = Required.DisallowNull)]
			[SuppressMessage("ReSharper", "UnusedMember.Local")]
			private string ContextIDString {
				get {
					return ContextID.ToString();
				}

				set {
					if (string.IsNullOrEmpty(value)) {
						return;
					}

					ulong result;
					if (!ulong.TryParse(value, out result)) {
						return;
					}

					ContextID = result;
				}
			}

			internal ulong AssetID { get; set; }

			[JsonProperty(PropertyName = "assetid", Required = Required.DisallowNull)]
			private string AssetIDString {
				get {
					return AssetID.ToString();
				}

				set {
					if (string.IsNullOrEmpty(value)) {
						return;
					}

					ulong result;
					if (!ulong.TryParse(value, out result)) {
						return;
					}

					AssetID = result;
				}
			}

			[JsonProperty(PropertyName = "id", Required = Required.DisallowNull)]
			[SuppressMessage("ReSharper", "UnusedMember.Local")]
			private string ID {
				get { return AssetIDString; }
				set { AssetIDString = value; }
			}

			internal ulong ClassID { get; set; }

			[JsonProperty(PropertyName = "classid", Required = Required.DisallowNull)]
			[SuppressMessage("ReSharper", "UnusedMember.Local")]
			private string ClassIDString {
				get {
					return ClassID.ToString();
				}

				set {
					if (string.IsNullOrEmpty(value)) {
						return;
					}

					ulong result;
					if (!ulong.TryParse(value, out result)) {
						return;
					}

					ClassID = result;
				}
			}

			internal ulong InstanceID { private get; set; }

			[JsonProperty(PropertyName = "instanceid", Required = Required.DisallowNull)]
			[SuppressMessage("ReSharper", "UnusedMember.Local")]
			private string InstanceIDString {
				get {
					return InstanceID.ToString();
				}

				set {
					if (string.IsNullOrEmpty(value)) {
						return;
					}

					ulong result;
					if (!ulong.TryParse(value, out result)) {
						return;
					}

					InstanceID = result;
				}
			}

			internal uint Amount { get; set; }

			[JsonProperty(PropertyName = "amount", Required = Required.Always)]
			[SuppressMessage("ReSharper", "UnusedMember.Local")]
			private string AmountString {
				get {
					return Amount.ToString();
				}

				set {
					if (string.IsNullOrEmpty(value)) {
						return;
					}

					uint result;
					if (!uint.TryParse(value, out result)) {
						return;
					}

					Amount = result;
				}
			}

			internal uint RealAppID { get; set; }
			internal EType Type { get; set; }
		}

		internal sealed class TradeOffer { // REF: https://developer.valvesoftware.com/wiki/Steam_Web_API/IEconService#CEcon_TradeOffer
			[SuppressMessage("ReSharper", "UnusedMember.Global")]
			internal enum ETradeOfferState : byte {
				Unknown,
				Invalid,
				Active,
				Accepted,
				Countered,
				Expired,
				Canceled,
				Declined,
				InvalidItems,
				EmailPending,
				EmailCanceled,
				OnHold
			}

			internal ulong TradeOfferID { get; set; }

			[JsonProperty(PropertyName = "tradeofferid", Required = Required.Always)]
			[SuppressMessage("ReSharper", "UnusedMember.Local")]
			private string TradeOfferIDString {
				get {
					return TradeOfferID.ToString();
				}

				set {
					if (string.IsNullOrEmpty(value)) {
						return;
					}

					ulong result;
					if (!ulong.TryParse(value, out result)) {
						return;
					}

					TradeOfferID = result;
				}
			}

			[JsonProperty(PropertyName = "accountid_other", Required = Required.Always)]
			internal uint OtherSteamID3 { private get; set; }

			[JsonProperty(PropertyName = "trade_offer_state", Required = Required.Always)]
			internal ETradeOfferState State { get; set; }

			[JsonProperty(PropertyName = "items_to_give", Required = Required.Always)]
			internal HashSet<Item> ItemsToGive { get; } = new HashSet<Item>();

			[JsonProperty(PropertyName = "items_to_receive", Required = Required.Always)]
			internal HashSet<Item> ItemsToReceive { get; } = new HashSet<Item>();

			// Extra
			internal ulong OtherSteamID64 => OtherSteamID3 == 0 ? 0 : new SteamID(OtherSteamID3, EUniverse.Public, EAccountType.Individual);

			internal bool IsSteamCardsOnlyTradeForUs() => ItemsToGive.All(item => (item.AppID == Item.SteamAppID) && (item.ContextID == Item.SteamContextID) && (item.Type == Item.EType.TradingCard));

			internal bool IsPotentiallyDupesTradeForUs() {
				Dictionary<uint, Dictionary<Item.EType, uint>> itemsToGivePerGame = new Dictionary<uint, Dictionary<Item.EType, uint>>();
				foreach (Item item in ItemsToGive) {
					Dictionary<Item.EType, uint> itemsPerType;
					if (!itemsToGivePerGame.TryGetValue(item.RealAppID, out itemsPerType)) {
						itemsPerType = new Dictionary<Item.EType, uint> { [item.Type] = item.Amount };
						itemsToGivePerGame[item.RealAppID] = itemsPerType;
					} else {
						uint amount;
						if (itemsPerType.TryGetValue(item.Type, out amount)) {
							itemsPerType[item.Type] = amount + item.Amount;
						} else {
							itemsPerType[item.Type] = item.Amount;
						}
					}
				}

				Dictionary<uint, Dictionary<Item.EType, uint>> itemsToReceivePerGame = new Dictionary<uint, Dictionary<Item.EType, uint>>();
				foreach (Item item in ItemsToReceive) {
					Dictionary<Item.EType, uint> itemsPerType;
					if (!itemsToReceivePerGame.TryGetValue(item.RealAppID, out itemsPerType)) {
						itemsPerType = new Dictionary<Item.EType, uint> { [item.Type] = item.Amount };
						itemsToReceivePerGame[item.RealAppID] = itemsPerType;
					} else {
						uint amount;
						if (itemsPerType.TryGetValue(item.Type, out amount)) {
							itemsPerType[item.Type] = amount + item.Amount;
						} else {
							itemsPerType[item.Type] = item.Amount;
						}
					}
				}

				// Ensure that amount of items to give is at least amount of items to receive (per game and per type)
				foreach (KeyValuePair<uint, Dictionary<Item.EType, uint>> itemsPerGame in itemsToGivePerGame) {
					Dictionary<Item.EType, uint> otherItemsPerType;
					if (!itemsToReceivePerGame.TryGetValue(itemsPerGame.Key, out otherItemsPerType)) {
						return false;
					}

					foreach (KeyValuePair<Item.EType, uint> itemsPerType in itemsPerGame.Value) {
						uint otherAmount;
						if (!otherItemsPerType.TryGetValue(itemsPerType.Key, out otherAmount)) {
							return false;
						}

						if (itemsPerType.Value > otherAmount) {
							return false;
						}
					}
				}

				return true;
			}
		}

		[SuppressMessage("ReSharper", "UnusedMember.Global")]
		internal sealed class TradeOfferRequest {
			internal sealed class ItemList {
				[JsonProperty(PropertyName = "assets", Required = Required.Always)]
				internal HashSet<Item> Assets { get; } = new HashSet<Item>();
			}

			[JsonProperty(PropertyName = "newversion", Required = Required.Always)]
			internal bool NewVersion { get; } = true;

			[JsonProperty(PropertyName = "version", Required = Required.Always)]
			internal byte Version { get; } = 2;

			[JsonProperty(PropertyName = "me", Required = Required.Always)]
			internal ItemList ItemsToGive { get; } = new ItemList();

			[JsonProperty(PropertyName = "them", Required = Required.Always)]
			internal ItemList ItemsToReceive { get; } = new ItemList();
		}

		[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
		internal sealed class ConfirmationResponse {
			[JsonProperty(PropertyName = "success", Required = Required.Always)]
			internal bool Success { get; private set; }
		}

		[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
		internal sealed class ConfirmationDetails {
			internal enum EType : byte {
				Unknown,
				Trade,
				Market,
				Other
			}

			internal uint ConfirmationID { get; set; }

			[JsonProperty(PropertyName = "success", Required = Required.Always)]
			internal bool Success { get; private set; }

			private EType _Type;
			private EType Type {
				get {
					if (_Type != EType.Unknown) {
						return _Type;
					}

					if (HtmlDocument == null) {
						return EType.Unknown;
					}

					HtmlNode testNode = HtmlDocument.DocumentNode.SelectSingleNode("//div[@class='mobileconf_listing_prices']");
					if (testNode != null) {
						_Type = EType.Market;
						return _Type;
					}

					testNode = HtmlDocument.DocumentNode.SelectSingleNode("//div[@class='mobileconf_trade_area']");
					if (testNode != null) {
						_Type = EType.Trade;
						return _Type;
					}

					_Type = EType.Other;
					return _Type;
				}
			}

			private ulong _TradeOfferID;
			internal ulong TradeOfferID {
				get {
					if (_TradeOfferID != 0) {
						return _TradeOfferID;
					}

					if ((Type != EType.Trade) || (HtmlDocument == null)) {
						return 0;
					}

					HtmlNode htmlNode = HtmlDocument.DocumentNode.SelectSingleNode("//div[@class='tradeoffer']");
					if (htmlNode == null) {
						Logging.LogNullError(nameof(htmlNode));
						return 0;
					}

					string id = htmlNode.GetAttributeValue("id", null);
					if (string.IsNullOrEmpty(id)) {
						Logging.LogNullError(nameof(id));
						return 0;
					}

					int index = id.IndexOf('_');
					if (index < 0) {
						Logging.LogNullError(nameof(index));
						return 0;
					}

					index++;
					if (id.Length <= index) {
						Logging.LogNullError(nameof(id.Length));
						return 0;
					}

					id = id.Substring(index);
					if (ulong.TryParse(id, out _TradeOfferID) && (_TradeOfferID != 0)) {
						return _TradeOfferID;
					}

					Logging.LogNullError(nameof(_TradeOfferID));
					return 0;
				}
			}

			private ulong _OtherSteamID64;
			internal ulong OtherSteamID64 {
				get {
					if (_OtherSteamID64 != 0) {
						return _OtherSteamID64;
					}

					if ((Type != EType.Trade) || (OtherSteamID3 == 0)) {
						return 0;
					}

					_OtherSteamID64 = new SteamID(OtherSteamID3, EUniverse.Public, EAccountType.Individual);
					return _OtherSteamID64;
				}
			}

			[JsonProperty(PropertyName = "html")]
			private string HTML;

			private uint _OtherSteamID3;
			private uint OtherSteamID3 {
				get {
					if (_OtherSteamID3 != 0) {
						return _OtherSteamID3;
					}

					if ((Type != EType.Trade) || (HtmlDocument == null)) {
						return 0;
					}

					HtmlNode htmlNode = HtmlDocument.DocumentNode.SelectSingleNode("//a/@data-miniprofile");
					if (htmlNode == null) {
						Logging.LogNullError(nameof(htmlNode));
						return 0;
					}

					string miniProfile = htmlNode.GetAttributeValue("data-miniprofile", null);
					if (string.IsNullOrEmpty(miniProfile)) {
						Logging.LogNullError(nameof(miniProfile));
						return 0;
					}

					if (uint.TryParse(miniProfile, out _OtherSteamID3) && (_OtherSteamID3 != 0)) {
						return _OtherSteamID3;
					}

					Logging.LogNullError(nameof(_OtherSteamID3));
					return 0;
				}
			}

			private HtmlDocument _HtmlDocument;
			private HtmlDocument HtmlDocument {
				get {
					if (_HtmlDocument != null) {
						return _HtmlDocument;
					}

					if (string.IsNullOrEmpty(HTML)) {
						return null;
					}

					_HtmlDocument = new HtmlDocument();
					_HtmlDocument.LoadHtml(WebUtility.HtmlDecode(HTML));
					return _HtmlDocument;
				}
			}
		}
	}
}
